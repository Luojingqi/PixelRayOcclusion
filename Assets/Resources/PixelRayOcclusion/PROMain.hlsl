#include "Random.hlsl"
#include "GetLine.hlsl"
const float PI = 3.14159265358979323846;


float4 RayOcclusion_Sun(int2 localPos, Sun sun)
{
    //像太阳的方向随机发射一条射线
    int2 sunEnd = GloabToLocal(sun.gloabPos) + int2(NextInt(-sun.r, sun.r), NextInt(-sun.r, sun.r));
    int2 sunLight[Line50];
    int sunIndex = GetLine_50(localPos, sunEnd, sunLight);
    
    //计算过滤颜色
    float4 filterColor = float4(0, 0, 0, 1);
    bool isShadow = false;
    for (int i = sunIndex - 1; i > 0; i--)
    {
        PixelColorInfo info = GetPixel(sunLight[i]);
        filterColor.xyz = filterColor.xyz * (1 - info.color.w) + info.color.xyz * info.color.w;
        filterColor.w *= (1 - info.color.w);
        if (info.color.w >= 1)
        {
            isShadow = true;
            break;
        }
        filterColor = saturate(filterColor);
    }
    //计算阴影
    float shadow = 1;
    if (isShadow || sunIndex == -1)
        shadow = 0;
    
    filterColor.xyz = sun.color.xyz * filterColor.w + filterColor.xyz * (1 - filterColor.w);
    filterColor.xyz *= pow(clamp(1 - distance(localPos, sunLight[sunIndex]) / Line50, 0, 1), 2); //光照强度2次项减小
    //filterColor.xyz *= (1 - distance(start_G, sunLight[sunIndex]) / SunRayLength); //光照强度均匀减小
    //filterColor.xyz *= 0.6f;
    return float4(filterColor.xyz, shadow);
}

