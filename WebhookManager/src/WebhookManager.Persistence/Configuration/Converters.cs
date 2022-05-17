using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebhookManager.Persistence.Configuration;

/// <summary>
/// Common Converters for Entity Type configuration
/// </summary>
public static class Converters
{
    public static ValueConverter<T, string> EnumValueConverter<T>()
        where T : Enum => new ValueConverter<T, string>(v => v.ToString(), v => EnumConverter<T>(v));
    public static T EnumConverter<T>(string value) => (T)Enum.Parse(typeof(T), value);
}
