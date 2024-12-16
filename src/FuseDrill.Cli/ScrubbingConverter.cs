using System.Text.Json;
using System.Text.Json.Serialization;

public class GuidScrubbingConverter : JsonConverter<Guid>
{
    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<Guid>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue("ScrubbedGuid");
    }
}

public class DateTimeScrubbingConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<DateTime>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue("ScrubbedDateTime");
    }

}

public class DateTimeOffsetScrubbingConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<DateTimeOffset>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue("ScrubbedDateTimeOffset");
    }

}

//Todo maybe do in this format:
//Use variable to increment
//{
//Id: Guid_1,
//  Name: In - Memory Test Data,
//  Date: DateTime_1,
//  Tags: [
//    example,
//    snapshot,
//    memory
//  ]
//}