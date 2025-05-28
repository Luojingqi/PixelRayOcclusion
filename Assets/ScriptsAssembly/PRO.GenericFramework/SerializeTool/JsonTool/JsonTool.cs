using Newtonsoft.Json;
using System.Text;
namespace PRO.Tool.Serialize.Json
{
    public static class JsonTool
    {
        private static JsonSerializerSettings writeSetting;
        private static JsonSerializerSettings readSetting;
        private static void Init()
        {
            writeSetting = new JsonSerializerSettings();
            writeSetting.Converters.Add(new JsonToolWriteEx());
            readSetting = new JsonSerializerSettings();
            readSetting.Converters.Add(new JsonToolReadEx());

        }

        public static string ToJson(object obj, bool indented = true)
        {
            if (writeSetting == null) Init();
            string jsonText = JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None, writeSetting);
            return jsonText;
        }

        public static byte[] ToJsonByteArray(object obj)
        {
            byte[] jsonByteArray = Encoding.UTF8.GetBytes(ToJson(obj));
            return jsonByteArray;
        }

        public static T ToObject<T>(string jsonText)
        {
            if (writeSetting == null) Init();
            return JsonConvert.DeserializeObject<T>(jsonText, readSetting);
        }

        public static bool ToObject<T>(string jsonText, out T obj)
        {
            if (writeSetting == null) Init();
            obj = JsonConvert.DeserializeObject<T>(jsonText, readSetting);
            return true;
        }

        public static void AddWriteJsonConverter(JsonConverter converter)
        {
            if (writeSetting == null) Init();
            writeSetting.Converters.Add(converter);
        }
        public static void AddReadJsonConverter(JsonConverter converter)
        {
            if (readSetting == null) Init();
            readSetting.Converters.Add(converter);
        }
    }
}