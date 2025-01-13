using PRO.Disk;
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
[StructLayout(LayoutKind.Sequential)]
public struct PixelColorInfoToShader
{
    /// <summary>
    /// ��ɫ
    /// </summary>
    public int4 color;
    /// <summary>
    /// ��ǿӰ���ϵ��
    /// </summary>
    public float affectsLightIntensity;
    /// <summary>
    /// �Է���ǿ��
    /// </summary>
    public float selfLuminous;

    public PixelColorInfoToShader(PixelColorInfo info)
    {
        color = new int4(info.color.r, info.color.g, info.color.b, info.color.a);
        affectsLightIntensity = info.affectsLightIntensity;
        selfLuminous = info.selfLuminous;
    }
};