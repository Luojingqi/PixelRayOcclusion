#include "StructData.hlsl"

//块数据
StructuredBuffer<BlockPixelInfo> BlockBuffer;

//点的光照颜色缓存
StructuredBuffer<int4> LightResultBuffer;

//点颜色属性
StructuredBuffer<PixelColorInfo> AllPixelColorInfo;


int2 UVToPixel(float2 uv)
{
    return int2(uv.x * BlockSizeY, uv.y * BlockSizeY);
}

int PixelToIndex(int2 pos)
{
    return pos.y * BlockSizeX + pos.x;
}

PixelColorInfo GetPixelColorInfo(int colorInfoId)
{
    return AllPixelColorInfo[colorInfoId];
}

BlockPixelInfo GetBlockPixelInfo(int index)
{
    return BlockBuffer[index];
}