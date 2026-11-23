using MediaBrowser.Model.Drawing;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Skia.Tests;

public class PercentPlayedDrawerTests
{
    [Fact]
    public void Process_ValidPercentage_DrawsOnCanvasWithoutErrors()
    {
        using var bitmap = new SKBitmap(200, 100);
        using var canvas = new SKCanvas(bitmap);
        var dimensions = new ImageDimensions(200, 100);

        PercentPlayedDrawer.Process(canvas, dimensions, 50.0);

        // Verify the canvas drew onto the bitmap without throwing and modified pixels
        canvas.Flush();
        Assert.NotEqual(SKColors.Empty, bitmap.GetPixel(10, 95));
    }

    [Fact]
    public void Process_ZeroPercentage_DrawsBackgroundRect()
    {
        using var bitmap = new SKBitmap(200, 100);
        using var canvas = new SKCanvas(bitmap);
        var dimensions = new ImageDimensions(200, 100);

        PercentPlayedDrawer.Process(canvas, dimensions, 0.0);

        canvas.Flush();
        Assert.NotEqual(SKColors.Empty, bitmap.GetPixel(50, 95));
    }

    [Fact]
    public void Process_FullPercentage_DrawsFullWidthRect()
    {
        using var bitmap = new SKBitmap(200, 100);
        using var canvas = new SKCanvas(bitmap);
        var dimensions = new ImageDimensions(200, 100);

        PercentPlayedDrawer.Process(canvas, dimensions, 100.0);

        canvas.Flush();
        // At 100%, foreground color (#FF00A4DC) should cover the bar
        var pixel = bitmap.GetPixel(100, 95);
        Assert.Equal(SKColor.Parse("#FF00A4DC"), pixel);
    }
}
