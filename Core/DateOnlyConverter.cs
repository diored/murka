using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DioRed.Murka.Core;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var dt))
        {
            return DateOnly.FromDateTime(dt);
        };

        var value = reader.GetString();
        if (value == null)
        {
            return default;
        }

        var match = DateOnlyCommon.DatePattern.Match(value);

        return match.Success ? DateOnlyCommon.FromMatch(match) : default;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}

public class DateOnlyNullableConverter : JsonConverter<DateOnly?>
{
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var dt))
        {
            return DateOnly.FromDateTime(dt);
        };

        var value = reader.GetString();
        if (value == null)
        {
            return default;
        }

        var match = DateOnlyCommon.DatePattern.Match(value);

        return match.Success ? DateOnlyCommon.FromMatch(match) : default;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString("yyyy-MM-dd"));
    }
}

public static class DateOnlyConverterExtensions
{
    public static void AddDateOnlyConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new DateOnlyConverter());
        options.Converters.Add(new DateOnlyNullableConverter());
    }
}

internal static class DateOnlyCommon
{
    internal static readonly Regex DatePattern = new(@"^(\d{4})-(\d{2})-(\d{2})(T|\s|\z)");

    internal static DateOnly FromMatch(Match match)
    {
        return new DateOnly(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
    }
}