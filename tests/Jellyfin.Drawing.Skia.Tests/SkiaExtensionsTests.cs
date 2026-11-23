using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Skia.Tests;

public class SkiaExtensionsTests
{
    [Fact]
    public void DrawBitmap_WithDestinationRect_DrawsSuccessfully()
    {
        using var canvasBitmap = new SKBitmap(100, 100);
        using var canvas = new SKCanvas(canvasBitmap);
        canvas.Clear(SKColors.Transparent);

        using var sourceBitmap = new SKBitmap(50, 50);
        sourceBitmap.Erase(SKColors.Blue);

        var destRect = new SKRect(10, 10, 60, 60);
        var samplingOptions = new SKSamplingOptions(SKFilterMode.Linear);

        canvas.DrawBitmap(sourceBitmap, destRect, samplingOptions);
        canvas.Flush();

        // Check that the blue rectangle was drawn within the destination bounds
        Assert.Equal(SKColors.Blue, canvasBitmap.GetPixel(30, 30));
    }

    [Fact]
    public void DrawBitmap_WithCoordinates_DrawsSuccessfully()
    {
        using var canvasBitmap = new SKBitmap(100, 100);
        using var canvas = new SKCanvas(canvasBitmap);
        canvas.Clear(SKColors.Transparent);

        using var sourceBitmap = new SKBitmap(40, 40);
        sourceBitmap.Erase(SKColors.Red);

        var samplingOptions = new SKSamplingOptions(SKFilterMode.Nearest);

        canvas.DrawBitmap(sourceBitmap, 20f, 20f, samplingOptions);
        canvas.Flush();

        // Check that the red bitmap was drawn at (20, 20)
        Assert.Equal(SKColors.Red, canvasBitmap.GetPixel(30, 30));
    }
}
