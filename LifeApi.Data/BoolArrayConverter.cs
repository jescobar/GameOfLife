using System.Text.Json;
using System.Text.Json.Serialization;

namespace LifeApi.Data;

public class BoolArrayConverter : JsonConverter<bool[,]>
{
    public override bool[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string jsonString = reader.GetString();
        List<List<bool>> list = JsonSerializer.Deserialize<List<List<bool>>>(jsonString);
        bool[,] array = new bool[list.Count, list[0].Count];

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[0].Count; j++)
            {
                array[i, j] = list[i][j];
            }
        }

        return array;
    }

    public override void Write(Utf8JsonWriter writer, bool[,] value, JsonSerializerOptions options)
    {
        int length0 = value.GetLength(0);
        int length1 = value.GetLength(1);
        List<List<bool>> list = new List<List<bool>>(length0);

        for (int i = 0; i < length0; i++)
        {
            List<bool> subList = new List<bool>(length1);

            for (int j = 0; j < length1; j++)
            {
                subList.Add(value[i, j]);
            }

            list.Add(subList);
        }

        string jsonString = JsonSerializer.Serialize(list);
        writer.WriteStringValue(jsonString);
    }
}