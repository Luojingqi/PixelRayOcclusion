using Google.FlatBuffers;

namespace PRO.Flat.Ex
{
    public static class BasicDataEx
    {
        public static Offset<Transform> ToDisk(this UnityEngine.Transform transform, FlatBufferBuilder builder)
        {
            var position = transform.position;
            var rotation = transform.rotation;
            var localScale = transform.localScale;
            return Transform.CreateTransform(builder,
                position.x, position.y, position.z,
                rotation.x, rotation.y, rotation.z, rotation.w,
                localScale.x, localScale.y, localScale.z);
        }
        public static void ToRAM(this UnityEngine.Transform transform, Transform value)
        {
            transform.position = value.Position.ToRAM();
            transform.rotation = value.Rotation.ToRAM();
            transform.localScale = value.LocalScale.ToRAM();
        }


        public static Offset<Rigidbody2D> ToDisk(this UnityEngine.Rigidbody2D rig2D, FlatBufferBuilder builder)
        {
            return Rigidbody2D.CreateRigidbody2D(builder, rig2D.velocity.x, rig2D.velocity.y, rig2D.angularVelocity, rig2D.simulated);
        }
        public static void ToRAM(this UnityEngine.Rigidbody2D rig2D, Rigidbody2D value)
        {
            rig2D.velocity = value.Velocity.ToRAM();
            rig2D.angularVelocity = value.AngularVelocity;
            rig2D.simulated = value.Simulated;
        }

        public static Offset<Vector2> ToDisk(this UnityEngine.Vector2 value, FlatBufferBuilder builder)
        {
            return Vector2.CreateVector2(builder, value.x, value.y);
        }
        public static UnityEngine.Vector2 ToRAM(this Vector2 value)
        {
            return new UnityEngine.Vector2(value.X, value.Y);
        }

        public static Offset<Vector3> ToDisk(this UnityEngine.Vector3 value, FlatBufferBuilder builder)
        {
            return Vector3.CreateVector3(builder, value.x, value.y, value.z);
        }
        public static UnityEngine.Vector3 ToRAM(this Vector3 value)
        {
            return new UnityEngine.Vector3(value.X, value.Y, value.Z);
        }

        public static Offset<Vector2Int> ToDisk(this UnityEngine.Vector2Int value, FlatBufferBuilder builder)
        {
            return Vector2Int.CreateVector2Int(builder, value.x, value.y);
        }
        public static UnityEngine.Vector2Int ToRAM(this Vector2Int value)
        {
            return new UnityEngine.Vector2Int(value.X, value.Y);
        }

        public static Offset<Vector3Int> ToDisk(this UnityEngine.Vector3Int value, FlatBufferBuilder builder)
        {
            return Vector3Int.CreateVector3Int(builder, value.x, value.y, value.z);
        }
        public static UnityEngine.Vector3Int ToRAM(this Vector3Int value)
        {
            return new UnityEngine.Vector3Int(value.X, value.Y, value.Z);
        }

        public static Offset<Quaternion> ToDisk(this UnityEngine.Quaternion quaternion, FlatBufferBuilder builder)
        {
            return Quaternion.CreateQuaternion(builder, quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
        public static UnityEngine.Quaternion ToRAM(this Quaternion value)
        {
            return new UnityEngine.Quaternion(value.X, value.Y, value.Z, value.W);
        }

        public static Offset<Vector2Byte> ToDisk(this PRO.DataStructure.Vector2Byte value, FlatBufferBuilder builder)
        {
            return Vector2Byte.CreateVector2Byte(builder, value.x, value.y);
        }
        public static DataStructure.Vector2Byte ToRAM(this Vector2Byte value)
        {
            return new DataStructure.Vector2Byte(value.X, value.Y);
        }

        public static Offset<Color32> ToDisk(this UnityEngine.Color32 color, FlatBufferBuilder builder)
        {
            return Color32.CreateColor32(builder, color.r, color.g, color.b, color.a);
        }
        public static UnityEngine.Color32 ToRAM(this Color32 value)
        {
            return new UnityEngine.Color32(value.R, value.G, value.B, value.A);
        }
    }
}
