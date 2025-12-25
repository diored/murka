namespace DioRed.Murka.Core.Entities;

public record Daily(string Id, string Quest, string Definition)
{
    public static Daily Weapon { get; } = new("W", "⚔️Оружие", "ПВ2 (Аурогон) / ПП / МИ / ГШ");
    public static Daily Armor { get; } = new("A", "🛡Доспех", "ПВ1 (Чернокрыл) / СЦ / ХХ 4-1 / ХХ 4-2 / ТС");
    public static Daily Relic { get; } = new("R", "💍Реликвия", "ХС / ЛА / ДР / ОР");

    public static Daily ByCode(string code) => code switch
    {
        "A" => Armor,
        "W" => Weapon,
        "R" => Relic,
        _ => new(code, string.Empty, "Unrecognized daily")
    };
}