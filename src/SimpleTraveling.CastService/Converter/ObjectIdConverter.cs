using System.Text.Json;
using System.Text.Json.Serialization;

using MongoDB.Bson;

namespace SimpleTraveling.CostService.Converter;

public class ObjectIdConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() is not { } value
        || string.IsNullOrWhiteSpace(value)
        || !ObjectId.TryParse(value, out var objectId)
            ? ObjectId.Empty : objectId;

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}
