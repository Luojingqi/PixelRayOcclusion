using PRO.Disk;
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
[StructLayout(LayoutKind.Sequential)]
public struct PixelColorInfoToShader
{
    /// <summary>
    /// 颜色
    /// </summary>
    public int4 color;
    /// <summary>
    /// 光强影响的系数
    /// </summary>
    public float affectsLightIntensity;
    /// <summary>
    /// 自发光强度
    /// </summary>
    public float selfLuminous;

    public PixelColorInfoToShader(PixelColorInfo info)
    {
        color = new int4(info.color.r, info.color.g, info.color.b, info.color.a);
        affectsLightIntensity = info.affectsLightIntensity;
        selfLuminous = info.selfLuminous;
    }
};