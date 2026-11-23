using System;
using System.IO;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Skia.Tests;

public class StripCollageBuilderTests
{
    [Theory]
    [InlineData("output.jpg", SKEncodedImageFormat.Jpeg)]
    [InlineData("output.JPEG", SKEncodedImageFormat.Jpeg)]
    [InlineData("output.webp", SKEncodedImageFormat.Webp)]
    [InlineData("output.gif", SKEncodedImageFormat.Gif)]
    [InlineData("output.bmp", SKEncodedImageFormat.Bmp)]
    [InlineData("output.png", SKEncodedImageFormat.Png)]
    [InlineData("output.unknown", SKEncodedImageFormat.Png)]
    public void GetEncodedFormat_ValidExtensions_ReturnsExpectedFormat(string fileName, SKEncodedImageFormat expected)
    {
        var result = StripCollageBuilder.GetEncodedFormat(fileName);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetEncodedFormat_NullPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StripCollageBuilder.GetEncodedFormat(null!));
    }

    [Fact]
    public void BuildSquareCollage_WithMockAndStubDependencies_CreatesCollageFile()
    {
        // -------------------------------------------------------------------------
        // Demonstrating Test Doubles:
        // 1. Mock: Mock<ILogger<SkiaEncoder>> verifies no error logs were dispatched.
        // 2. Stub: Mock<IApplicationPaths> configured to return a predetermined TempDirectory.
        // -------------------------------------------------------------------------
        var mockLogger = new Mock<ILogger<SkiaEncoder>>();
        var stubAppPaths = new Mock<IApplicationPaths>();
        stubAppPaths.Setup(p => p.TempDirectory).Returns(Path.GetTempPath());

        var encoder = new SkiaEncoder(mockLogger.Object, stubAppPaths.Object);
        var builder = new StripCollageBuilder(encoder);

        var tempOutput = Path.Combine(Path.GetTempPath(), $"square_{Guid.NewGuid():N}.png");

        try
        {
            builder.BuildSquareCollage(Array.Empty<string>(), tempOutput, 64, 64);

            Assert.True(File.Exists(tempOutput));
            Assert.True(new FileInfo(tempOutput).Length > 0);

            // Mock verification: verify logger was not invoked with errors
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }
        finally
        {
            if (File.Exists(tempOutput))
            {
                File.Delete(tempOutput);
            }
        }
    }

    [Fact]
    public void BuildThumbCollage_WithStubApplicationPaths_CreatesThumbCollageFile()
    {
        // Demonstrating Test Double:
        // Stub providing canned TempDirectory response
        var stubAppPaths = new Mock<IApplicationPaths>();
        stubAppPaths.Setup(p => p.TempDirectory).Returns(Path.GetTempPath());

        var mockLogger = new Mock<ILogger<SkiaEncoder>>();
        var encoder = new SkiaEncoder(mockLogger.Object, stubAppPaths.Object);
        var builder = new StripCollageBuilder(encoder);

        var tempOutput = Path.Combine(Path.GetTempPath(), $"thumb_{Guid.NewGuid():N}.jpg");

        try
        {
            builder.BuildThumbCollage(Array.Empty<string>(), tempOutput, 80, 60, "My Library");

            Assert.True(File.Exists(tempOutput));
            Assert.True(new FileInfo(tempOutput).Length > 0);
        }
        finally
        {
            if (File.Exists(tempOutput))
            {
                File.Delete(tempOutput);
            }
        }
    }
}
