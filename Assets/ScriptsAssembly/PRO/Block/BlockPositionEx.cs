using PRO.DataStructure;
using UnityEngine;

namespace PRO
{
    public static class BlockPositionEx
    {
        public static Vector2Int WorldToGlobal(this Vector2 worldPos) => Block.WorldToGlobal(worldPos);
        public static Vector2Int WorldToBlock(this Vector2 worldPos) => Block.WorldToBlock(worldPos);
        public static Vector2Byte WorldToPixel(this Vector3 worldPos) => Block.WorldToPixel(worldPos);

        public static Vector3 BlockToWorld(this Vector2Int blockPos) => Block.BlockToWorld(blockPos);

        public static Vector2Byte GlobalToPixel(this Vector2Int globalPos) => Block.GlobalToPixel(globalPos);
        public static Vector2Int GlobalToBlock(this Vector2Int globalPos) => Block.GlobalToBlock(globalPos);
        public static Vector3 GlobalToWorld(this Vector2Int globalPos) => Block.GlobalToWorld(globalPos);
    }
}
