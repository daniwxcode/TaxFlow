using Core.Domain.Tax.Penalties;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;
using System.Text.Json;

namespace TaxFlow.Infrastructure.Persistence.Configurations;

internal static class DurationConversion
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    internal static readonly ValueConverter<Duration, string> JsonConverter = new(
        value => Serialize(value),
        json => Deserialize(json));

    internal static readonly ValueComparer<Duration> JsonComparer = new(
        (left, right) => string.Equals(Serialize(left), Serialize(right), StringComparison.Ordinal),
        value => StringComparer.Ordinal.GetHashCode(Serialize(value)),
        value => Deserialize(Serialize(value)));

    private static string Serialize(Duration value)
    {
        var payload = new DurationPayload
        {
            Value = value.Value,
            Unit = value.Unit
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static Duration Deserialize(string json)
    {
        var payload = JsonSerializer.Deserialize<DurationPayload>(json, JsonOptions);
        if (payload is null)
            return Duration.Zero;

        return new Duration(payload.Value, payload.Unit);
    }

    private sealed class DurationPayload
    {
        public int Value { get; set; }
        public TimeUnit Unit { get; set; }
    }
}