namespace Server.Infrastructure.Extensions
{
    using System.Text.Json;

    public static class ObjectExtension
    {
        public static string ToJsonSerialize(this object @object) => JsonSerializer.Serialize(@object, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}