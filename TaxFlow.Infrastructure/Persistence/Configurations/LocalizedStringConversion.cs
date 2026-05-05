using Core.Domain.Localization;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal static class LocalizedStringConversion
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    internal static readonly ValueConverter<LocalizedString?, string?> JsonConverter = new(
        value => Serialize(value),
        json => Deserialize(json));

    internal static readonly ValueComparer<LocalizedString?> JsonComparer = new(
        (left, right) => string.Equals(Serialize(left), Serialize(right), StringComparison.Ordinal),
        value => Serialize(value)?.GetHashCode(StringComparison.Ordinal) ?? 0,
        value => Deserialize(Serialize(value)));

    private static string? Serialize(LocalizedString? value)
    {
        if (value is null) return null;

        var payload = new LocalizedStringPayload
        {
            Default = value.Default,
            Translations = new Dictionary<string, string>(value.Translations, StringComparer.OrdinalIgnoreCase)
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static LocalizedString? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        var payload = JsonSerializer.Deserialize<LocalizedStringPayload>(json, JsonOptions);
        if (payload is null) return null;

        var localized = LocalizedString.Create(payload.Default ?? string.Empty);
        if (payload.Translations is null) return localized;

        foreach (var (culture, value) in payload.Translations)
        {
            if (string.IsNullOrWhiteSpace(culture)) continue;
            localized.With(culture, value ?? string.Empty);
        }

        return localized;
    }

    private sealed class LocalizedStringPayload
    {
        public string? Default { get; set; }
        public Dictionary<string, string>? Translations { get; set; }
    }
}