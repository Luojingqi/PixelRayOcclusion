using PRO.Data;
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
[StructLayout(LayoutKind.Sequential)]
public struct PixelColorInfoToShader
{
    /// <summary>
    /// 颜色
    /// </summary>
    public int4 color;
    /// <summary>
    /// 是否影响光照
    /// </summary>
    public int affectsLight;
    /// <summary>
    /// 自发光强度
    /// </summary>
    public float selfLuminous;


    public static string sign_SL = "self";
    public PixelColorInfoToShader(PixelColorInfo info)
    {
        color = new int4(info.color.r, info.color.g, info.color.b, info.color.a);
        affectsLight = info.affectsLight ? 1 : 0;
        selfLuminous = 0;
        if (info.lightSourceType != null && info.lightSourceType.StartsWith(sign_SL))
        {
            string sl = info.lightSourceType.Substring(sign_SL.Length + 1, info.lightSourceType.Length - sign_SL.Length - 1);
            selfLuminous = Convert.ToSingle(sl);
        }
    }
};