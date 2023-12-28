using System.ComponentModel.DataAnnotations;

namespace DioRed.Murka.Manager.Models;

public class PromocodeModel
{
    [Required(ErrorMessage = "Поле обязательно к заполнению")]
    public string Code { get; set; } = default!;

    [RegularExpression(@"\d{4}-\d{1,2}-\d{1,2}(?: \d{1,2}:\d{1,2})?", ErrorMessage = "Поддерживаемые форматы: yyy-MM-dd и yyyy-MM-dd HH:mm")]
    public string? ValidFrom { get; set; }

    [RegularExpression(@"\d{4}-\d{1,2}-\d{1,2}(?: \d{1,2}:\d{1,2})?", ErrorMessage = "Поддерживаемые форматы: yyy-MM-dd и yyyy-MM-dd HH:mm")]
    public string? ValidTo { get; set; }

    [Required(ErrorMessage = "Поле обязательно к заполнению")]
    public string Content { get; set; } = default!;

    public PromocodeModel Clone() => new()
    {
        Code = Code,
        ValidFrom = ValidFrom,
        ValidTo = ValidTo,
        Content = Content
    };
}