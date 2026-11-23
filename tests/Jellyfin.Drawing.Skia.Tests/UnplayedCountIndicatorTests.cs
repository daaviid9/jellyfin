using MediaBrowser.Model.Drawing;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Skia.Tests;

public class UnplayedCountIndicatorTests
{
    [Fact]
    public void DrawUnplayedCountIndicator_SingleDigit_RendersWithoutErrors()
    {
        using var bitmap = new SKBitmap(300, 300);
        using var canvas = new SKCanvas(bitmap);
        var dimensions = new ImageDimensions(300, 300);

        UnplayedCountIndicator.DrawUnplayedCountIndicator(canvas, dimensions, 7);

        canvas.Flush();
        // Circle center is at (300 - 38, 38) = (262, 38). Check that pixels were drawn.
        Assert.NotEqual(SKColors.Empty, bitmap.GetPixel(262, 38));
    }

    [Fact]
    public void DrawUnplayedCountIndicator_TwoDigits_RendersWithoutErrors()
    {
        using var bitmap = new SKBitmap(300, 300);
        using var canvas = new SKCanvas(bitmap);
        var dimensions = new ImageDimensions(300, 300);

        UnplayedCountIndicator.DrawUnplayedCountIndicator(canvas, dimensions, 42);

        canvas.Flush();
        Assert.NotEqual(SKColors.Empty, bitmap.GetPixel(262, 38));
    }

    [Fact]
    public void DrawUnplayedCountIndicator_ThreeOrMoreDigits_RendersWithoutErrors()
    {
        using var bitmap = new SKBitmap(300, 300);
        using var canvas = new SKCanvas(bitmap);
        var dimensions = new ImageDimensions(300, 300);

        UnplayedCountIndicator.DrawUnplayedCountIndicator(canvas, dimensions, 150);

        canvas.Flush();
        Assert.NotEqual(SKColors.Empty, bitmap.GetPixel(262, 38));
    }
}
