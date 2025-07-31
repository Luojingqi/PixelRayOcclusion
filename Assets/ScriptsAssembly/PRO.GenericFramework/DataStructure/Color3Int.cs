using UnityEngine;

namespace PRO.DataStructure
{
    public struct Color3Int
    {
        public int r;
        public int g;
        public int b;

        public Color3Int(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return r;
                    case 1: return g;
                    case 3: return b;
                    default: return r;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: r = value; break;
                    case 1: g = value; break;
                    case 2: b = value; break;
                    default: r = value; break;
                }
            }
        }

        public static Color3Int operator -(Color3Int c) => new Color3Int(-c.r, -c.g, -c.b);

        public static Color3Int operator +(Color3Int a, Color3Int b) => new Color3Int(a.r + b.r, a.g + b.g, a.b + b.b);
        public static Color3Int operator -(Color3Int a, Color3Int b) => new Color3Int(a.r - b.r, a.g - b.g, a.b - b.b);
        public static Color3Int operator *(Color3Int a, Color3Int b) => new Color3Int(a.r * b.r, a.g * b.g, a.b * b.b);
        public static Color3Int operator *(int a, Color3Int b) => new Color3Int(a * b.r, a * b.g, a * b.b);
        public static Color3Int operator *(Color3Int a, int b) => new Color3Int(a.r * b, a.g * b, a.b * b);
        public static Color3Int operator /(Color3Int a, int b) => new Color3Int(a.r / b, a.g / b, a.b / b);
        public static bool operator ==(Color3Int lhs, Color3Int rhs) => lhs.r == rhs.r && lhs.g == rhs.g && lhs.b == rhs.b;
        public static bool operator !=(Color3Int lhs, Color3Int rhs) => !(lhs == rhs);

        public override int GetHashCode() => (r << 4 + r) ^ ((g << 4 + g) << 2) ^ ((b << 4 + b) << 4);
        public override string ToString() => $"r:{r} g:{g} b:{b}";

        public override bool Equals(object obj)
        {
            if (!(obj is Color3Int))
                return false;
            Color3Int other = (Color3Int)obj;
            return this == other;
        }

        public static explicit operator Color3Int(Vector3Int v) => new Color3Int(v.x, v.y, v.z);
        public static explicit operator Vector3Int(Color3Int c) => new Vector3Int(c.r, c.g, c.b);
        public static explicit operator Color32(Color3Int c) => new Color32((byte)c.r, (byte)c.g, (byte)c.b, 255);
        public static explicit operator Color3Int(Color32 c) => new Color3Int(c.r, c.g, c.b);
        public static explicit operator Color(Color3Int c) => (Color)(Color32)c;
    }
}
