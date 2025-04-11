#include "StructData.hlsl"

//������
StructuredBuffer<BlockPixelInfo> BlockBuffer;

//��Ĺ�����ɫ����
StructuredBuffer<int4> LightResultBuffer;

//����ɫ����
StructuredBuffer<PixelColorInfo> AllPixelColorInfo;


int2 UVToPixel(float2 uv)
{
    return min(int2(uv.x * BlockSizeY, uv.y * BlockSizeY), int2(BlockSizeX - 1, BlockSizeY - 1));
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