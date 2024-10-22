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
int GetBlockBuffer(int buffer, int index)
{
    switch (buffer)
    {
        default:
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
    }
}
RWStructuredBuffer<int4> LightBufferTemp;
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

int2 GloabToPixel(int2 gloabPos)
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
    int index = ReceiveLightToBlockIndex(rlPos);
    return AllPixelColorInfo[GetBlockBuffer(index, PixelToIndex(ReceiveLightToPixel(rlPos)))];
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