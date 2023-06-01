using SkiaSharp;

namespace DioRed.Murkal.ForecastBuilder;

internal static class Constants
{
    public static class Canvas
    {
        public static SKColor Background { get; } = new(102, 102, 102);
        public static int ItemWidth { get; } = 68;
        public static int MinimumWidth { get; } = 220;
        public static int Height { get; } = 200;
    }

    public static class Today
    {
        public static SKColor Background { get; } = new(64, 64, 64);
        public static SKSize Size { get; } = new(58, 122);
        public static int VerticalPadding { get; } = 4;
    }

    public static class DayNumber
    {
        public static SKColor Color { get; } = new(230, 230, 230);
        public static SKTypeface Typeface { get; } = SKTypeface.FromFamilyName("PT Sans", SKFontStyle.Bold);
        public static int FontSize { get; } = 36;
        public static int Top { get; } = 36;
    }

    public static class Month
    {
        public static SKColor Color { get; } = new(230, 230, 230);
        public static SKTypeface Typeface { get; } = SKTypeface.FromFamilyName("PT Sans");
        public static int FontSize { get; } = 16;
        public static int Top { get; } = 54;
    }

    public static class Icon
    {
        public static int Top { get; } = 95;
        public static SKSize Size { get; } = new(48, 48);
    }

    public static class Legend
    {
        public static SKPoint Position { get; } = new(17, 130);
        public static SKSize IconSize { get; } = new(20, 20);
        public static SKColor TextColor { get; } = new(230, 230, 230);
        public static SKTypeface Typeface { get; } = SKTypeface.FromFamilyName("PT Sans");
        public static int FontSize { get; } = 12;
        public static int Margin { get; } = 3;
        public static SKPoint TextOffset { get; } = new(23, 14);
    }

    public static class Images
    {
        public static SKBitmap Weapon { get; } = SKBitmap.Decode("Assets\\DPSRole.png");
        public static SKBitmap Armor { get; } = SKBitmap.Decode("Assets\\TankRole.png");
        public static SKBitmap Relic { get; } = SKBitmap.Decode("Assets\\HealerRole.png");
    }

    public static class DailyDescription
    {
        public static string Weapon { get; } = "Оружие: ПВ−2, ПП, МИ, ГШ";
        public static string Armor { get; } = "Доспех: ПВ−1, СЦ, ХХ 4−1, 4−2";
        public static string Relic { get; } = "Реликвия: ХС, ЛА, ДР, ОР";
    }
}
