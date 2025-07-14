namespace PRO.Proto.Ex
{
    public static class BasicDataEx
    {
        public static UnityEngine.Vector2 ToRAM(this PRO.Proto.Vector2 value) => new UnityEngine.Vector2(value.X, value.Y);
        public static PRO.Proto.Vector2 ToDisk(this UnityEngine.Vector2 value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Vector2>();
            ret.X = value.x;
            ret.Y = value.y;
            return ret;
        }
        public static UnityEngine.Vector3 ToRAM(this PRO.Proto.Vector3 value) => new UnityEngine.Vector3(value.X, value.Y, value.Z);
        public static PRO.Proto.Vector3 ToDisk(this UnityEngine.Vector3 value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Vector3>();
            ret.X = value.x;
            ret.Y = value.y;
            ret.Z = value.z;
            return ret;
        }

        public static PRO.DataStructure.Vector2Byte ToRAM(this PRO.Proto.Vector2Byte value) => new PRO.DataStructure.Vector2Byte(value.X, value.Y);
        public static PRO.Proto.Vector2Byte ToDisk(this PRO.DataStructure.Vector2Byte value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Vector2Byte>();
            ret.X = value.x;
            ret.Y = value.y;
            return ret;
        }

        public static UnityEngine.Vector2Int ToRAM(this PRO.Proto.Vector2Int value) => new UnityEngine.Vector2Int(value.X, value.Y);
        public static PRO.Proto.Vector2Int ToDisk(this UnityEngine.Vector2Int value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Vector2Int>();
            ret.X = value.x;
            ret.Y = value.y;
            return ret;
        }
        public static UnityEngine.Vector3Int ToRAM(this PRO.Proto.Vector3Int value) => new UnityEngine.Vector3Int(value.X, value.Y, value.Z);
        public static PRO.Proto.Vector3Int ToDisk(this UnityEngine.Vector3Int value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Vector3Int>();
            ret.X = value.x;
            ret.Y = value.y;
            ret.Z = value.z;
            return ret;
        }

        public static UnityEngine.Quaternion ToRAM(this PRO.Proto.Quaternion value) => new UnityEngine.Quaternion(value.X, value.Y, value.Z, value.W);
        public static PRO.Proto.Quaternion ToDisk(this UnityEngine.Quaternion value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Quaternion>();
            ret.X = value.x;
            ret.Y = value.y;
            ret.Z = value.z;
            ret.W = value.w;
            return ret;
        }
        public static UnityEngine.Color32 ToRAM(this PRO.Proto.Color32 value) => new UnityEngine.Color32((byte)value.R, (byte)value.G, (byte)value.B, (byte)value.A);
        public static PRO.Proto.Color32 ToDisk(this UnityEngine.Color32 value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Color32>();
            ret.R = value.r;
            ret.G = value.g;
            ret.B = value.b;
            ret.A = value.a;
            return ret;
        }


        public static void ToRAM(this UnityEngine.Transform transform, Transform value)
        {
            transform.position = value.Position.ToRAM();
            transform.rotation = value.Rotation.ToRAM();
            transform.localScale = value.LocalScale.ToRAM();
        }
        public static Transform ToDisk(this UnityEngine.Transform transform)
        {
            var p = Transform.Parser;
            var ret = ProtoPool.TakeOut<PRO.Proto.Transform>();
            ret.Position = transform.position.ToDisk();
            ret.Rotation = transform.rotation.ToDisk();
            ret.LocalScale = transform.localScale.ToDisk();
            return ret;
        }

        public static void ToRAM(this UnityEngine.Rigidbody2D rigidbody2D, Rigidbody2D value)
        {
            rigidbody2D.velocity = value.Velocity.ToRAM();
            rigidbody2D.angularVelocity = value.AngularVelocity;
            rigidbody2D.simulated = value.Simulated;
        }
        public static Rigidbody2D ToDisk(this UnityEngine.Rigidbody2D rigidbody2D)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.Rigidbody2D>();
            ret.Velocity = rigidbody2D.velocity.ToDisk();
            ret.AngularVelocity = rigidbody2D.angularVelocity;
            ret.Simulated = rigidbody2D.simulated;
            return ret;
        }

        public static UnityEngine.RaycastHit2D ToRAM(this RaycastHit2D value)
        {
            var ret = new UnityEngine.RaycastHit2D();
            ret.centroid = value.Centroid.ToRAM();
            ret.distance = value.Distance;
            ret.fraction = value.Fraction;
            ret.normal = value.Normal.ToRAM();
            ret.point = value.Point.ToRAM();
            return ret;
        }

        public static RaycastHit2D ToDisk(this UnityEngine.RaycastHit2D value)
        {
            var ret = ProtoPool.TakeOut<PRO.Proto.RaycastHit2D>();
            ret.Centroid = value.centroid.ToDisk();
            ret.Distance = value.distance;
            ret.Fraction = value.fraction;
            ret.Normal = value.normal.ToDisk();
            ret.Point = value.point.ToDisk();
            return ret;
        }
    }
}
