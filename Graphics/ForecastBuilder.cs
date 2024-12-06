using SkiaSharp;

namespace DioRed.Murka.Graphics;

public static class ForecastBuilder
{
    public static ReadOnlySpan<byte> BuildImage(ScheduleItem[] schedule)
    {
        using SKFont dayNumberFont = new()
        {
            Typeface = Constants.DayNumber.Typeface,
            Size = Constants.DayNumber.FontSize
        };

        using SKPaint dayNumberPaint = new()
        {
            Color = Constants.DayNumber.Color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using SKFont monthFont = new()
        {
            Typeface = Constants.Month.Typeface,
            Size = Constants.Month.FontSize
        };

        using SKPaint monthPaint = new()
        {
            Color = Constants.Month.Color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using SKFont legendFont = new()
        {
            Typeface = Constants.Legend.Typeface,
            Size = Constants.Legend.FontSize
        };

        using SKPaint legendPaint = new()
        {
            Color = Constants.Legend.TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using SKPaint backgroundPaint = new()
        {
            Color = Constants.Today.Background
        };

        int width = Math.Max(Constants.Canvas.MinimumWidth, Constants.Canvas.ItemWidth * schedule.Length);
        int height = Constants.Canvas.Height;

        SKImageInfo info = new(width, height);

        using var surface = SKSurface.Create(info);

        var canvas = surface.Canvas;

        canvas.Clear(Constants.Canvas.Background);

        SKRect todayRect = new()
        {
            Left = (Constants.Canvas.ItemWidth - Constants.Today.Size.Width) / 2,
            Top = Constants.Today.VerticalPadding,
            Size = Constants.Today.Size
        };

        canvas.DrawRoundRect(
            rect: todayRect,
            r: new SKSize(4, 4),
            paint: backgroundPaint
        );

        for (int i = 0; i < schedule.Length; i++)
        {
            float x = Constants.Canvas.ItemWidth * (i + 0.5f);

            canvas.DrawText(
                text: schedule[i].Day,
                x: x,
                y: Constants.DayNumber.Top,
                textAlign: SKTextAlign.Center,
                font: dayNumberFont,
                paint: dayNumberPaint);

            canvas.DrawText(
                text: schedule[i].Month,
                x: x,
                y: Constants.Month.Top,
                textAlign: SKTextAlign.Center,
                font: monthFont,
                paint: monthPaint
            );

            SKRect rect = new()
            {
                Left = x - Constants.Icon.Size.Width / 2,
                Top = Constants.Icon.Top - Constants.Icon.Size.Height / 2,
                Size = Constants.Icon.Size
            };

            var dailyImage = schedule[i].Quest switch
            {
                "W" => Constants.Images.Weapon,
                "A" => Constants.Images.Armor,
                "R" => Constants.Images.Relic,
                var q => throw new InvalidOperationException($"Unknown quest code: {q}")
            };

            canvas.DrawBitmap(dailyImage, rect);
        }

        var legendItems = new[]
        {
            (Icon: Constants.Images.Weapon, Text: Constants.DailyDescription.Weapon),
            (Icon: Constants.Images.Armor, Text: Constants.DailyDescription.Armor),
            (Icon: Constants.Images.Relic, Text: Constants.DailyDescription.Relic)
        };

        for (int i = 0; i < legendItems.Length; i++)
        {
            canvas.DrawBitmap(
                bitmap: legendItems[i].Icon,
                dest: new SKRect
            {
                Location = Constants.Legend.Position + new SKPoint(0, i * (Constants.Legend.IconSize.Height + Constants.Legend.Margin)),
                Size = Constants.Legend.IconSize
            });

            canvas.DrawText(
                text: legendItems[i].Text,
                p: Constants.Legend.Position + Constants.Legend.TextOffset + new SKPoint(0, i * (Constants.Legend.IconSize.Height + Constants.Legend.Margin)),
                textAlign: SKTextAlign.Left,
                font: legendFont,
                paint: legendPaint
            );
        }

        using var image = surface.Snapshot();

        SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.AsSpan();
    }
}