using UnityEngine;

namespace PRO.DataStructure
{
    public struct Color4Int
    {
        public int r;
        public int g;
        public int b;
        public int a;

        public Color4Int(int r, int g, int b, int a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        // 索引器（0:r, 1:g, 2:b, 3:a）
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return r;
                    case 1: return g;
                    case 2: return b;
                    case 3: return a;
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
                    case 3: a = value; break;
                    default: r = value; break;
                }
            }
        }

        public static Color4Int operator -(Color4Int c) => new Color4Int(-c.r, -c.g, -c.b, -c.a);
        public static Color4Int operator +(Color4Int a, Color4Int b) => new Color4Int(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        public static Color4Int operator -(Color4Int a, Color4Int b) => new Color4Int(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        public static Color4Int operator *(Color4Int a, Color4Int b) => new Color4Int(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        public static Color4Int operator *(int scalar, Color4Int c) => new Color4Int(scalar * c.r, scalar * c.g, scalar * c.b, scalar * c.a);
        public static Color4Int operator *(Color4Int c, int scalar) => scalar * c;
        public static Color4Int operator /(Color4Int c, int divisor) => new Color4Int(c.r / divisor, c.g / divisor, c.b / divisor, c.a / divisor);
        public static bool operator ==(Color4Int lhs, Color4Int rhs) => lhs.r == rhs.r && lhs.g == rhs.g && lhs.b == rhs.b && lhs.a == rhs.a;
        public static bool operator !=(Color4Int lhs, Color4Int rhs) => !(lhs == rhs);

        public override int GetHashCode() => (r << 12) ^ (g << 8) ^ (b << 4) ^ a;
        public override string ToString() => $"r:{r} g:{g} b:{b} a:{a}";

        public override bool Equals(object obj) => obj is Color4Int other && this == other;

        public static explicit operator Color4Int(Vector3Int v) => new Color4Int(v.x, v.y, v.z, 255);
        public static explicit operator Vector3Int(Color4Int c) => new Vector3Int(c.r, c.g, c.b);
        public static explicit operator Color32(Color4Int c) => new Color32((byte)c.r, (byte)c.g, (byte)c.b, (byte)c.a);
        public static explicit operator Color4Int(Color32 c) => new Color4Int(c.r, c.g, c.b, c.a);
        public static explicit operator Color(Color4Int c) => (Color)(Color32)c;
    }
}