using DioRed.Murka.Core.Modules;

namespace DioRed.Murka.Infrastructure.Modules;

public class GreetingModule : IGreetingModule
{
    public string GetRandomGreeting()
    {
        string[] greetings =
        [
            "Привет! =)",
            "Мяу :-)",
            ",,,==(^.^)==,,,",
            "Рада видеть ;)",
            "И вам здравствуйте!",
            "Мурр",
            "Чао )",
            "Фрррр"
        ];

        return greetings[Random.Shared.Next(greetings.Length)];
    }
}