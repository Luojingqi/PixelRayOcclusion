using PRO.DataStructure;
using UnityEngine;
namespace PRO
{
    public static class MousePoint
    {
        public static Vector2 worldPos;
        public static Vector2Int blockPos;
        public static Vector2Int globalPos;
        public static Vector2Byte pixelPos;
        public static Block block;
        public static BackgroundBlock backgroundBlock;
        public static void Update()
        {
            Vector3 m = Input.mousePosition;
            m.z = 1;
            worldPos = Camera.main.ScreenToWorldPoint(m);
            globalPos = Block.WorldToGlobal(worldPos);
            blockPos = Block.GlobalToBlock(globalPos);
            pixelPos = Block.GlobalToPixel(globalPos);
            block = SceneManager.Inst.NowScene.GetBlock(blockPos);
            backgroundBlock = SceneManager.Inst.NowScene.GetBackground(blockPos);
        }
    }
}
