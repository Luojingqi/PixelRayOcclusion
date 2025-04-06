using UnityEngine;

namespace PRO.DataStructure
{
    public struct Vector2Byte
    {
        public byte x;
        public byte y;
        public Vector2Byte(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2Byte(int x, int y)
        {
            this.x = (byte)x;
            this.y = (byte)y;
        }
        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default: return x;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    default: x = value; break;
                }
            }
        }
        public float magnitude => Mathf.Sqrt(x * x + y * y);

        public static Vector2Byte operator -(Vector2Byte v) => new Vector2Byte(-v.x, -v.y);
        public static Vector2Byte operator +(Vector2Byte a, Vector2Byte b) => new Vector2Byte(a.x + b.x, a.y + b.y);
        public static Vector2Int operator +(Vector2Byte a, Vector2Int b) => new Vector2Int(a.x + b.x, a.y + b.y);
        public static Vector2Int operator +(Vector2Int b, Vector2Byte a) => new Vector2Int(a.x + b.x, a.y + b.y);
        public static Vector2Byte operator -(Vector2Byte a, Vector2Byte b) => new Vector2Byte(a.x - b.x, a.y - b.y);
        public static Vector2Int operator -(Vector2Byte a, Vector2Int b) => new Vector2Int(a.x + b.x, a.y + b.y);
        public static Vector2Int operator -(Vector2Int b, Vector2Byte a) => new Vector2Int(a.x + b.x, a.y + b.y);
        public static Vector2Byte operator *(Vector2Byte a, Vector2Byte b) => new Vector2Byte(a.x * b.x, a.y * b.y);
        public static Vector2Byte operator *(int a, Vector2Byte b) => new Vector2Byte(a * b.x, a * b.y);
        public static Vector2Byte operator *(Vector2Byte a, int b) => new Vector2Byte(a.x * b, a.y * b);
        public static Vector2Byte operator /(Vector2Byte a, int b) => new Vector2Byte(a.x / b, a.y / b);
        public static bool operator ==(Vector2Byte lhs, Vector2Byte rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
        public static bool operator !=(Vector2Byte lhs, Vector2Byte rhs) => !(lhs == rhs);
        public override int GetHashCode() => (x << 4 + x) ^ ((y << 4 + y) << 2);
        public override string ToString() => $"x:{x} y:{y}";

        public static explicit operator Vector2(Vector2Byte v) => new Vector2(v.x, v.y);
        public static explicit operator Vector2Int(Vector2Byte v) => new Vector2Int(v.x, v.y);
        public static explicit operator Vector3(Vector2Byte v) => new Vector3(v.x, v.y);
        public static explicit operator Vector2Byte(Vector2Int v) => new Vector2Byte((byte)v.x, (byte)v.y);

        public static Vector2Byte max = new Vector2Byte(byte.MaxValue, byte.MaxValue);
        public static Vector2Byte min = new Vector2Byte(byte.MinValue, byte.MinValue);
    }
}
