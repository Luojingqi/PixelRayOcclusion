using PRO.DataStructure;
using UnityEngine;
namespace PRO.Tool
{
    public struct DrawPixelTask
    {
        public Vector2Byte pos;
        public Color32 color;
        public DrawPixelTask(Vector2Byte pos, Color32 color)
        {
            this.pos = pos;
            this.color = color;
        }
    }
    /// <summary>
    /// 绘制图形任务
    /// </summary>
    public class DrawGraphTaskData
    { }
    public class DrawGraph_Line : DrawGraphTaskData
    {
        public Vector2Int pos_G0;
        public Vector2Int pos_G1;
        public Color32 color;
    }

    public class DrawGraph_Ring : DrawGraphTaskData
    {
        public Vector2Int pos_G;
        public int r;
        public Color32 color;
    }

    public class DrawGraph_Circle : DrawGraphTaskData
    {
        public Vector2Int pos_G;
        public int r;
        public Color32 color;
    }

    public class DrawGraph_Polygon : DrawGraphTaskData
    {
        public Vector2Int pos_G;
        public int r;
        public int n;
        public float rotate;
        public Color32 color;
    }

    public class DrawGraph_Octagon : DrawGraphTaskData
    {
        public Vector2Int pos_G;
        public int r;
        public Color32 color;
    }
}