#include "StructData.hlsl"

#define BlockSizeX 110
#define BlockSizeY 110

StructuredBuffer<int> BlockBuffer0;
StructuredBuffer<int> BlockBuffer1;
StructuredBuffer<int> BlockBuffer2;
StructuredBuffer<int> BlockBuffer3;
StructuredBuffer<int> BlockBuffer4;
StructuredBuffer<int> BlockBuffer5;
StructuredBuffer<int> BlockBuffer6;
StructuredBuffer<int> BlockBuffer7;
StructuredBuffer<int> BlockBuffer8;
StructuredBuffer<int> BlockBuffer9;
StructuredBuffer<int> BlockBuffer10;
StructuredBuffer<int> BlockBuffer11;
StructuredBuffer<int> BlockBuffer12;
StructuredBuffer<int> BlockBuffer13;
StructuredBuffer<int> BlockBuffer14;
StructuredBuffer<int> BlockBuffer15;
StructuredBuffer<int> BlockBuffer16;
StructuredBuffer<int> BlockBuffer17;
StructuredBuffer<int> BlockBuffer18;
StructuredBuffer<int> BlockBuffer19;
StructuredBuffer<int> BlockBuffer20;
StructuredBuffer<int> BlockBuffer21;
StructuredBuffer<int> BlockBuffer22;
StructuredBuffer<int> BlockBuffer23;
StructuredBuffer<int> BlockBuffer24;

int GetBlockBuffer(int buffer, int index)
{
    switch (buffer)
    {
        default:
            return BlockBuffer0[index];
        case 0:
            return BlockBuffer0[index];
        case 1:
            return BlockBuffer1[index];
        case 2:
            return BlockBuffer2[index];
        case 3:
            return BlockBuffer3[index];
        case 4:
            return BlockBuffer4[index];
        case 5:
            return BlockBuffer5[index];
        case 6:
            return BlockBuffer6[index];
        case 7:
            return BlockBuffer7[index];
        case 8:
            return BlockBuffer8[index];
        case 9:
            return BlockBuffer9[index];
        case 10:
            return BlockBuffer10[index];
        case 11:
            return BlockBuffer11[index];
        case 12:
            return BlockBuffer12[index];
        case 13:
            return BlockBuffer13[index];
        case 14:
            return BlockBuffer14[index];
        case 15:
            return BlockBuffer15[index];
        case 16:
            return BlockBuffer16[index];
        case 17:
            return BlockBuffer17[index];
        case 18:
            return BlockBuffer18[index];
        case 19:
            return BlockBuffer19[index];
        case 20:
            return BlockBuffer20[index];
        case 21:
            return BlockBuffer21[index];
        case 22:
            return BlockBuffer22[index];
        case 23:
            return BlockBuffer23[index];
        case 24:
            return BlockBuffer24[index];
        
        
    }
}

RWStructuredBuffer<float4> LightBuffer;
//点颜色属性
StructuredBuffer<PixelColorInfo> AllPixelColorInfo;

//太阳
StructuredBuffer<Sun> SunBuffer;

//每个区块接收多大范围内的光照
int2 EachBlockReceiveLightSize;
//当前计算的是哪个区块的光照
int2 BlockPos;

#define _sun SunBuffer[0]

//本地坐标，在一个接收光照的范围内，最左下角为原点

int2 GloabToLocal(int2 gloabPos)
{
    int2 minReceiveLightBlockPos = BlockPos - (EachBlockReceiveLightSize / 2);
    return int2(gloabPos.x - minReceiveLightBlockPos.x * BlockSizeX, gloabPos.y - minReceiveLightBlockPos.y * BlockSizeY);
}

int2 IDToLocal(int2 id)
{
    return int2(id.x + (EachBlockReceiveLightSize.x / 2) * BlockSizeX, id.y + (EachBlockReceiveLightSize.y / 2) * BlockSizeY);
}

int2 LocalToPixel(int2 localPos)
{
    return int2(localPos.x % BlockSizeX, localPos.y % BlockSizeY);
}

//本地坐标转区块的索引
int LocalToIndex(int2 localPos)
{
    return localPos.x / BlockSizeX + localPos.y / BlockSizeY * EachBlockReceiveLightSize.x;
}

int PixelToIndex(int2 pos)
{
    return pos.y * BlockSizeX + pos.x;
}

bool Equalsf4(float4 f0, float4 f1)
{
    return f0.x == f1.x && f0.y == f1.y && f0.z == f1.z && f0.w == f1.w;
}
bool Equalsi2(int2 i0, int2 i1)
{
    return i0.x == i1.x && i0.y == i1.y;
}



//根据全局坐标返回点信息
PixelColorInfo GetPixel(int2 localPos)
{
    int index = LocalToIndex(localPos);
    return AllPixelColorInfo[GetBlockBuffer(index, PixelToIndex(LocalToPixel(localPos)))];
}