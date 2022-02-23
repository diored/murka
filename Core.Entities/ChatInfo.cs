﻿namespace DioRed.Murka.Core.Entities;

public record ChatInfo(string Id, string Type, string Title)
{
    public override string ToString() => $"{Type}:{Id}";
}