using UnityEngine;

namespace PRO.Tool
{
    public static class Log
    {
        public static void Print(string text, Color32 color)
        {
            Debug.Log($"<color={color.ToHex()}>{text}</color>");
        }
        public static void Print(string text)
        {
            Debug.Log($"<color={new Color32(200, 200, 200, 0).ToHex()}>{text}</color>");
        }

        public static string ToHex(this Color32 color)
        {
            return $"#{color.r:X2}{color.g:X2}{color.b:X2}";
        }
    }
}
