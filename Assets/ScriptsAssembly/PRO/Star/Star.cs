using PRO;
using PRO.Tool;
using System;
using UnityEngine;

public class Star
{
    public int r;
    public int zeroGR;
    public float g;
    public Vector2Int GloabPos;
    public static Vector2[] normals = new Vector2[] { new(0, -1), new(-1, -1), new(-1, 0), new(-1, 1), new(0, 1), new(1, 1), new(1, 0), new(1, -1) };

    public Star(Vector2Int GloabPos, int r, int zeroGR, string name, float g = 10)
    {
        if (r < zeroGR) new Exception("零重力区不能大于星球半径");
        this.r = r;
        this.zeroGR = zeroGR;
        this.g = g;
        this.GloabPos = GloabPos;
        var list = DrawTool.GetOctagon(GloabPos, r);
        foreach (var pos_G in list)
        {
            var block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(pos_G));
            Pixel pixel = Pixel.TakeOut(name, 0, Block.GlobalToPixel(pos_G));
            block.SetPixel(pixel);
            block.DrawPixelAsync();
        }
    }

    /// <summary>
    /// 获取此星球对某点的加速度
    /// </summary>
    public Vector3 GetAcceleration(Vector2Int gloabPos)
    {
        Vector2Int D = gloabPos - GloabPos;
        float angle = Mathf.Atan2(D.y, D.x) - Mathf.PI / 8;
        if (angle < 0) angle += 2 * Mathf.PI;
        int i = (int)(angle / (Mathf.PI / 4));
        i = (8 - i + 1) % 8;
        float d = D.magnitude;
        if (d >= r)
        {
            //if ((2 * r - d) <= 0) return Vector3.zero;
            return Mathf.Pow((2 * r - d) / r, 2) * normals[i] * g * Pixel.Size;
        }
        else if (d >= zeroGR)
        {
            return d / r * normals[i] * g * Pixel.Size;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
