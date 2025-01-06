#include "../StructData.hlsl"
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
int GetBlockBuffer(int buffer, int Index)
{
    switch (buffer)
    {
        default:
case 0:
 return BlockBuffer0[Index];
case 1:
 return BlockBuffer1[Index];
case 2:
 return BlockBuffer2[Index];
case 3:
 return BlockBuffer3[Index];
case 4:
 return BlockBuffer4[Index];
case 5:
 return BlockBuffer5[Index];
case 6:
 return BlockBuffer6[Index];
case 7:
 return BlockBuffer7[Index];
case 8:
 return BlockBuffer8[Index];
case 9:
 return BlockBuffer9[Index];
case 10:
 return BlockBuffer10[Index];
case 11:
 return BlockBuffer11[Index];
case 12:
 return BlockBuffer12[Index];
case 13:
 return BlockBuffer13[Index];
case 14:
 return BlockBuffer14[Index];
case 15:
 return BlockBuffer15[Index];
case 16:
 return BlockBuffer16[Index];
case 17:
 return BlockBuffer17[Index];
case 18:
 return BlockBuffer18[Index];
case 19:
 return BlockBuffer19[Index];
case 20:
 return BlockBuffer20[Index];
case 21:
 return BlockBuffer21[Index];
case 22:
 return BlockBuffer22[Index];
case 23:
 return BlockBuffer23[Index];
case 24:
 return BlockBuffer24[Index];
    }
}
RWStructuredBuffer<int4> LightResultBufferTemp;
//点颜色属性
StructuredBuffer<PixelColorInfo> AllPixelColorInfo;
//光源
StructuredBuffer<LightSource> LightSourceBuffer;

//每个区块接收多大范围内的光照
int2 EachBlockReceiveLightSize;
//当前计算的是哪个区块的光照
int2 BlockPos;


//接收光照坐标，在当前区块的接收光照的范围内，最左下角为原点
int2 GloabToReceiveLight(int2 gloabPos)
{
    int2 minReceiveLightBlockPos = BlockPos - (EachBlockReceiveLightSize / 2);
    return int2(gloabPos.x - minReceiveLightBlockPos.x * BlockSizeX, gloabPos.y - minReceiveLightBlockPos.y * BlockSizeY);
}

int2 IDToReceiveLight(int2 id)
{
    return int2(id.x + (EachBlockReceiveLightSize.x / 2) * BlockSizeX, id.y + (EachBlockReceiveLightSize.y / 2) * BlockSizeY);
}

int2 ReceiveLightToPixel(int2 pos)
{
    return int2(pos.x % BlockSizeX, pos.y % BlockSizeY);
}

int2 GlobalToPixel(int2 gloabPos)
{
    return int2(gloabPos.x - BlockPos.x * BlockSizeX, gloabPos.y - BlockPos.y * BlockSizeY);
}

//本地坐标转区块的索引
int ReceiveLightToBlockIndex(int2 pos)
{
    return pos.x / BlockSizeX + pos.y / BlockSizeY * EachBlockReceiveLightSize.x;
}

int PixelToIndex(int2 pos)
{
    return pos.y * BlockSizeX + pos.x;
}

int2 GlockToBlock(int2 gloabPos)
{
    if (gloabPos.x < 0)
        gloabPos.x -= BlockSizeX - 1;
    if (gloabPos.y < 0)
        gloabPos.y -= BlockSizeY - 1;
    return int2(gloabPos.x / BlockSizeX, gloabPos.y / BlockSizeY);
}

bool Equalsf4(float4 f0, float4 f1)
{
    return f0.x == f1.x && f0.y == f1.y && f0.z == f1.z && f0.w == f1.w;
}
bool Equalsi2(int2 i0, int2 i1)
{
    return i0.x == i1.x && i0.y == i1.y;
}

//根据本地坐标返回点信息
PixelColorInfo GetPixel(int2 gloabPos)
{
    int2 rlPos = GloabToReceiveLight(gloabPos);
    int Index = ReceiveLightToBlockIndex(rlPos);
    return AllPixelColorInfo[GetBlockBuffer(Index, PixelToIndex(ReceiveLightToPixel(rlPos)))];
}

int2 beMixed_Min;
int2 beMixed_Max;

int2 IDToGloabPos(int2 id)
{
    int2 ret;
    switch (id.y)
    {
        default:
        case 0:
            ret = int2(id.x + beMixed_Min.x, beMixed_Min.y);
            break;
        case 1:
            ret = int2(id.x + beMixed_Min.x, beMixed_Max.y);
            break;
        case 2:
            ret = int2(beMixed_Min.x, id.x + beMixed_Min.y);
            break;
        case 3:
            ret = int2(beMixed_Max.x, id.x + beMixed_Min.y);
            break;
    }
    ret.x = max(ret.x, beMixed_Min.x);
    ret.x = min(ret.x, beMixed_Max.x);
    ret.y = max(ret.y, beMixed_Min.y);
    ret.y = min(ret.y, beMixed_Max.y);
    return ret;
}