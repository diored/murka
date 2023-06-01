using SkiaSharp;

namespace DioRed.Murkal.ForecastBuilder;

public static class ForecastBuilder
{
    public static ReadOnlySpan<byte> BuildImage(ScheduleItem[] schedule)
    {
        SKPaint dayNumberPaint = new()
        {
            Color = Constants.DayNumber.Color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Center,
            Typeface = Constants.DayNumber.Typeface,
            TextSize = Constants.DayNumber.FontSize
        };

        SKPaint monthPaint = new()
        {
            Color = Constants.Month.Color,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Center,
            Typeface = Constants.Month.Typeface,
            TextSize = Constants.Month.FontSize
        };

        SKPaint legendPaint = new()
        {
            Color = Constants.Legend.TextColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Typeface = Constants.Legend.Typeface,
            TextSize = Constants.Legend.FontSize
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
        canvas.DrawRoundRect(todayRect, new SKSize(4, 4), new SKPaint() { Color = Constants.Today.Background });

        for (int i = 0; i < schedule.Length; i++)
        {
            float x = Constants.Canvas.ItemWidth * (i + 0.5f);

            canvas.DrawText(schedule[i].Day, x, Constants.DayNumber.Top, dayNumberPaint);
            canvas.DrawText(schedule[i].Month, x, Constants.Month.Top, monthPaint);

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
            canvas.DrawBitmap(legendItems[i].Icon, new SKRect
            {
                Location = Constants.Legend.Position + new SKPoint(0, i * (Constants.Legend.IconSize.Height + Constants.Legend.Margin)),
                Size = Constants.Legend.IconSize
            });
            canvas.DrawText(legendItems[i].Text, Constants.Legend.Position + Constants.Legend.TextOffset + new SKPoint(0, i * (Constants.Legend.IconSize.Height + Constants.Legend.Margin)), legendPaint);
        }

        using var image = surface.Snapshot();

        SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.AsSpan();
    }
}
