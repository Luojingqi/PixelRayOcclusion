#include "StructData.hlsl"

//������
StructuredBuffer<int> BlockBuffer;

//��Ĺ�����ɫ����
StructuredBuffer<int4> LightBuffer;

//����ɫ����
StructuredBuffer<PixelColorInfo> AllPixelColorInfo;


int2 UVToPixel(float2 uv)
{
    return int2(uv.x * BlockSizeY, uv.y * BlockSizeY);
}

int PixelToIndex(int2 pos)
{
    return pos.y * BlockSizeX + pos.x;
}

PixelColorInfo GetPixel(int2 pixelPos)
{
    return AllPixelColorInfo[BlockBuffer[PixelToIndex(pixelPos)]];
}