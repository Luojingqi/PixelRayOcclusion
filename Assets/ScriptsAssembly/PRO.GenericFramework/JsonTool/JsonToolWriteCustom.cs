using UnityEngine;
using Unity.Plastic.Newtonsoft.Json;
using System;
using Unity.Plastic.Newtonsoft.Json.Linq;
namespace PRO.Tool
{
    /// <summary>
    /// LitJson自定义扩展
    /// </summary>
    internal class JsonToolWriteCustom : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jsonObject = new JObject();
            if (value is Vector4)
            {
                var v = (Vector4)value;
                jsonObject.Add("x", v.x);
                jsonObject.Add("y", v.y);
                jsonObject.Add("z", v.z);
                jsonObject.Add("w", v.w);
            }
            else if (value is Vector3)
            {
                var v = (Vector3)value;
                jsonObject.Add("x", v.x);
                jsonObject.Add("y", v.y);
                jsonObject.Add("z", v.z);
            }
            else if (value is Vector2)
            {
                var v = (Vector2)value;
                jsonObject.Add("x", v.x);
                jsonObject.Add("y", v.y);
            }
            else if (value is Vector3Int)
            {
                var v = (Vector3Int)value;
                jsonObject.Add("x", v.x);
                jsonObject.Add("y", v.y);
                jsonObject.Add("z", v.z);
            }
            else if (value is Vector2Int)
            {
                var v = (Vector2Int)value;
                jsonObject.Add("x", v.x);
                jsonObject.Add("y", v.y);
            }
            else if (value is Color32)
            {
                var v = (Color32)value;
                jsonObject.Add("r", v.r);
                jsonObject.Add("g", v.g);
                jsonObject.Add("b", v.b);
                jsonObject.Add("a", v.a);
            }
            else if (value is Color)
            {
                var v = (Color)value;
                jsonObject.Add("r", v.r);
                jsonObject.Add("g", v.g);
                jsonObject.Add("b", v.b);
                jsonObject.Add("a", v.a);
            }
            jsonObject.WriteTo(writer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector4)
                || objectType == typeof(Vector3)
                || objectType == typeof(Vector2)
                || objectType == typeof(Vector3Int)
                || objectType == typeof(Vector2Int)
                || objectType == typeof(Color32)
                || objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}