using PRO.DataStructure;
using UnityEngine;
namespace PRO
{
    public static class MousePoint
    {
        /// <summary>
        /// 鼠标坐标指向上一帧的向量
        /// </summary>
        public static Vector2 mousePosD;
        /// <summary>
        /// 鼠标坐标上一帧
        /// </summary>
        public static Vector2 mousePosLast;
        /// <summary>
        /// 鼠标坐标
        /// </summary>
        public static Vector2 mousePos;
        public static Vector2 worldPos;
        public static Vector2Int blockPos;
        public static Vector2Int globalPos;
        public static Vector2Byte pixelPos;
        public static Block block;
        public static BackgroundBlock backgroundBlock;
        public static void Update(SceneEntity scene)
        {
            mousePosLast = mousePos;
            mousePos = Input.mousePosition;
            mousePosD = mousePos - mousePosLast;
            worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1));
            globalPos = Block.WorldToGlobal(worldPos);
            blockPos = Block.GlobalToBlock(globalPos);
            pixelPos = Block.GlobalToPixel(globalPos);
            block = scene?.GetBlock(blockPos);
            backgroundBlock = scene?.GetBackground(blockPos);
        }
    }
}
