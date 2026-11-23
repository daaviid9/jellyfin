using System;
using System.Diagnostics;
using Xunit;
using Xunit.v3;

namespace Jellyfin.Extensions.Tests;

public class StringExtensionsProfilingTests
{
    private static readonly string[] SampleTitles =
    [
        "The Lord of the Rings: The Fellowship of the Ring (2001) [Extended Edition] [1080p]!",
        "Amélie: Le Fabuleux Destin d'Amélie Poulain (2001)",
        "Spider-Man: Across the Spider-Verse (2023) [WEBDL-2160p] [HDR] [5.1]",
        "Björk - Post (Live in Paris & London) [Remastered]...",
        "Cidadão Kane / Citizen Kane (1941) [Special 70th Anniversary Edition]!!!",
        "Kieślowski - Three Colours: Blue / Blanc / Rouge (1993-1994)",
        "Le cœur a ses raisons - S01E05 - L'amour en fuite (720p)",
        "Breaking Bad: S05E16 - Felina [1080p.BluRay.x264-ROVERS]",
        "WALL·E (2008) [Criterion Collection #1161]",
        "  Neon Genesis Evangelion: The End of Evangelion (新世紀エヴァンゲリオン劇場版)  "
    ];

    private readonly ITestOutputHelper _output;

    public StringExtensionsProfilingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("The Matrix (1999) [1080p]!", "the matrix 1999 1080p")]
    [InlineData("Amélie: Le Fabuleux Destin d'Amélie Poulain", "amelie le fabuleux destin d amelie poulain")]
    [InlineData("Spider-Man: No Way Home", "spider man no way home")]
    [InlineData("Björk", "bjork")]
    [InlineData("Cidadão Kane", "cidadao kane")]
    [InlineData("   ", "   ")]
    [InlineData("", "")]
    public void GetCleanValue_ValidatesCorrectness(string input, string expected)
    {
        var result = input.GetCleanValue();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetCleanValue_ProfilingScenario_RunsBenchmark()
    {
        const int WarmupIterations = 5_000;
        const int MeasuredIterations = 100_000;
        long checksum = 0;

        // Warmup JIT
        for (int i = 0; i < WarmupIterations; i++)
        {
            foreach (var title in SampleTitles)
            {
                checksum += title.GetCleanValue().Length;
            }
        }

        // Force GC before measurement for clean baseline
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long startBytes = GC.GetTotalAllocatedBytes(true);
        int startGen0 = GC.CollectionCount(0);
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < MeasuredIterations; i++)
        {
            foreach (var title in SampleTitles)
            {
                checksum += title.GetCleanValue().Length;
            }
        }

        stopwatch.Stop();
        long endBytes = GC.GetTotalAllocatedBytes(true);
        int endGen0 = GC.CollectionCount(0);

        long totalAllocatedBytes = endBytes - startBytes;
        int totalGen0Collections = endGen0 - startGen0;
        double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
        int totalOperations = MeasuredIterations * SampleTitles.Length;

        // Print human-readable output
        _output.WriteLine("[PROFILING RESULT]");
        _output.WriteLine($"Total Operations: {totalOperations:N0}");
        _output.WriteLine($"Elapsed Time:     {elapsedMilliseconds:N2} ms");
        _output.WriteLine($"Allocated Memory: {totalAllocatedBytes / (1024.0 * 1024.0):N2} MB ({totalAllocatedBytes:N0} bytes)");
        _output.WriteLine($"Gen0 Collections: {totalGen0Collections}");
        _output.WriteLine($"Per Operation:    {elapsedMilliseconds * 1000.0 / totalOperations:N2} µs, {totalAllocatedBytes / (double)totalOperations:N1} B");
        _output.WriteLine($"Checksum:         {checksum}");

        Assert.True(elapsedMilliseconds > 0 && checksum > 0);
    }
}
