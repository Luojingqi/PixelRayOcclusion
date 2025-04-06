using PRO.DataStructure;
using UnityEngine;
namespace PRO
{
    public static class MousePoint
    {
        public static Vector2 mousePosD;
        public static Vector2 mousePosLast;
        public static Vector2 mousePos;
        public static Vector2 worldPos;
        public static Vector2Int blockPos;
        public static Vector2Int globalPos;
        public static Vector2Byte pixelPos;
        public static Block block;
        public static BackgroundBlock backgroundBlock;
        public static void Update()
        {
            mousePosLast = mousePos;
            mousePos = Input.mousePosition;
            mousePosD = mousePos - mousePosLast;
            worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1));
            globalPos = Block.WorldToGlobal(worldPos);
            blockPos = Block.GlobalToBlock(globalPos);
            pixelPos = Block.GlobalToPixel(globalPos);
            block = SceneManager.Inst.NowScene.GetBlock(blockPos);
            backgroundBlock = SceneManager.Inst.NowScene.GetBackground(blockPos);
        }
    }
}
