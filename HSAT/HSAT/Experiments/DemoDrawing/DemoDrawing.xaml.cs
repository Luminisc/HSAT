using HSAT.Core;
using Microsoft.Maui.Platform;
using OSGeo.GDAL;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace HSAT.Experiments.DemoDrawing;

public partial class DemoDrawing : ContentPage
{
    DemoImageFile Image { get; set; }
    SKBitmap Bitmap { get; set; }
    float Scale { get; set; } = 1.0f;
    SKPoint Translate { get; set; } = new SKPoint(0, 0);
    SKPoint PrevTranslate { get; set; } = new SKPoint(0, 0);

    public DemoDrawing()
    {
        InitializeComponent();
    }

    private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;
        int width = e.Info.Width;
        int height = e.Info.Height;

        if (Image != null)
        {
            canvas.Clear();

            if (this.Bitmap == null)
            {
                var sw = new Stopwatch();
                sw.Start();
                var band = Image.Dataset.GetRasterBand(140);
                var bandWidth = band.XSize;
                var bandHeight = band.YSize;
                var bitmap = new SKBitmap(bandWidth, bandHeight);
                var datatype = band.DataType;
                var buffer = new short[bandWidth * bandHeight];
                var readResult = band.ReadRaster(0, 0, bandWidth, bandHeight, buffer, bandWidth, bandHeight, 0, 0);
                Debug.WriteLine($"Read band in: {sw.ElapsedMilliseconds} ms");
                sw.Restart();
                if (readResult != CPLErr.CE_None)
                {
                    Debug.WriteLine("Oh no");
                    bitmap.Dispose();
                    return;
                }

                band.ComputeStatistics(false, out var min, out var max, out var mean, out var stddev, null, null);
                var pixelsPtr = bitmap.GetPixels();
                unsafe
                {
                    var unsafePtr = (uint*)pixelsPtr.ToPointer();
                    for (var y = 0; y < bandHeight; y++)
                    {
                        for (var x = 0; x < bandWidth; x++)
                        {
                            int color = buffer[y * bandWidth + x];
                            if (color < 0)
                                color = 0;
                            var colorb = (byte)(256 * color / max);
                            unsafePtr[y * bandWidth + x] = (uint)new SKColor(colorb, colorb, colorb);
                        }
                    }
                }

                sw.Stop();
                Debug.WriteLine($"Loaded in: {sw.ElapsedMilliseconds} ms");

                this.Bitmap = bitmap;
            }

            int resizedWidth = (int)(this.Bitmap.Width * Scale),
                resizedHeight = (int)(this.Bitmap.Height * Scale);
                

            using (var resizedBitmap = this.Bitmap.Resize(new SKImageInfo(resizedWidth, resizedHeight), SKFilterQuality.None))
            {
                canvas.DrawBitmap(resizedBitmap, Translate);
            }
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Image = DemoImageFile.Load();
        skCanvas.InvalidateSurface();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
#if WINDOWS
        var view = skCanvas.Handler.PlatformView as SkiaSharp.Views.Windows.SKXamlCanvas;
        view.PointerWheelChanged += (s, e) =>
        {
            Scale += e.GetCurrentPoint(null).Properties.MouseWheelDelta / 1000.0f;
            skCanvas.InvalidateSurface();
        };
#endif
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                Translate = PrevTranslate + new SKPoint((float)e.TotalX, (float)e.TotalY);
                break;
            case GestureStatus.Completed:
                PrevTranslate = Translate;
                break;
            case GestureStatus.Canceled:
                Translate = PrevTranslate;
                break;
        }
        skCanvas.InvalidateSurface();
    }
}