using System.Text.Json;

using SimpleTraveling.Test.Helpers;

using Xunit.Abstractions;

namespace SimpleTraveling.Test;

public static class ContextExtensions
{
    public static T WriteLineObject<T>(this ITestOutputHelper testOutputHelper, T t)
    {
        testOutputHelper.WriteLine(JsonSerializer.Serialize(t, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new ObjectIdConverter()
            }
        }));
        return t;
    }
}
