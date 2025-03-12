#include "../StructData.hlsl"
StructuredBuffer<BlockPixelInfo> BlockBuffer0;
StructuredBuffer<BlockPixelInfo> BlockBuffer1;
StructuredBuffer<BlockPixelInfo> BlockBuffer2;
StructuredBuffer<BlockPixelInfo> BlockBuffer3;
StructuredBuffer<BlockPixelInfo> BlockBuffer4;
StructuredBuffer<BlockPixelInfo> BlockBuffer5;
StructuredBuffer<BlockPixelInfo> BlockBuffer6;
StructuredBuffer<BlockPixelInfo> BlockBuffer7;
StructuredBuffer<BlockPixelInfo> BlockBuffer8;
StructuredBuffer<BlockPixelInfo> BlockBuffer9;
StructuredBuffer<BlockPixelInfo> BlockBuffer10;
StructuredBuffer<BlockPixelInfo> BlockBuffer11;
StructuredBuffer<BlockPixelInfo> BlockBuffer12;
StructuredBuffer<BlockPixelInfo> BlockBuffer13;
StructuredBuffer<BlockPixelInfo> BlockBuffer14;
StructuredBuffer<BlockPixelInfo> BlockBuffer15;
StructuredBuffer<BlockPixelInfo> BlockBuffer16;
StructuredBuffer<BlockPixelInfo> BlockBuffer17;
StructuredBuffer<BlockPixelInfo> BlockBuffer18;
StructuredBuffer<BlockPixelInfo> BlockBuffer19;
StructuredBuffer<BlockPixelInfo> BlockBuffer20;
StructuredBuffer<BlockPixelInfo> BlockBuffer21;
StructuredBuffer<BlockPixelInfo> BlockBuffer22;
StructuredBuffer<BlockPixelInfo> BlockBuffer23;
StructuredBuffer<BlockPixelInfo> BlockBuffer24;
BlockPixelInfo GetBlockBuffer(int buffer, int Index)
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
int2 GloabToReceiveLight(int2 globalPos)
{
    int2 minReceiveLightBlockPos = BlockPos - (EachBlockReceiveLightSize / 2);
    return int2(globalPos.x - minReceiveLightBlockPos.x * BlockSizeX, globalPos.y - minReceiveLightBlockPos.y * BlockSizeY);
}

int2 IDToReceiveLight(int2 id)
{
    return int2(id.x + (EachBlockReceiveLightSize.x / 2) * BlockSizeX, id.y + (EachBlockReceiveLightSize.y / 2) * BlockSizeY);
}

int2 ReceiveLightToPixel(int2 pos)
{
    return int2(pos.x % BlockSizeX, pos.y % BlockSizeY);
}

int2 GlobalToPixel(int2 globalPos)
{
    return int2(globalPos.x - BlockPos.x * BlockSizeX, globalPos.y - BlockPos.y * BlockSizeY);
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

int2 GlockToBlock(int2 globalPos)
{
    if (globalPos.x < 0)
        globalPos.x -= BlockSizeX - 1;
    if (globalPos.y < 0)
        globalPos.y -= BlockSizeY - 1;
    return int2(globalPos.x / BlockSizeX, globalPos.y / BlockSizeY);
}

bool Equalsf4(float4 f0, float4 f1)
{
    return f0.x == f1.x && f0.y == f1.y && f0.z == f1.z && f0.w == f1.w;
}
bool Equalsi2(int2 i0, int2 i1)
{
    return i0.x == i1.x && i0.y == i1.y;
}


PixelColorInfo GetPixelColorInfo(int colorInfoId)
{
    return AllPixelColorInfo[colorInfoId];
}
//根据全局坐标返回点信息
BlockPixelInfo GetBlockPixelInfo(int2 globalPos)
{
    int2 rlPos = GloabToReceiveLight(globalPos);
    int Index = ReceiveLightToBlockIndex(rlPos);
    return GetBlockBuffer(Index, PixelToIndex(ReceiveLightToPixel(rlPos)));
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