namespace Server.Infrastructure.Extensions
{
    using System.Text.Json;

    public static class StringExtension
    {
        public static T ToJsonDeserialize<T>(this string value) => JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}