using PRO;
using PRO.Data;
using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PROEditorTool
{

    [MenuItem("PRO/1.����ExcelTool")]
    public static void StartExcelToolEXE()
    {
        ExcelTool.ExcelMain.Start();
    }
    [MenuItem("PRO/2.����HLSL�ļ�")]
    public static void AutoCreateHLSL()
    {
        if (!JsonTool.LoadingText(Application.streamingAssetsPath + @"\Json\LightSourceInfo.json", out string lightSourceInfoText))
        {
            Debug.Log("����ʧ��");
            return;
        }
        LightSourceInfo[] array = JsonTool.ToObject<LightSourceInfo[]>(lightSourceInfoText);

        Dictionary<string, LightSourceInfo> lightSourceInfoDic = new Dictionary<string, LightSourceInfo>();
        List<LightSourceInfo> lightSourceInfoList = new List<LightSourceInfo>();
        #region ����·���µ�����LightSourceInfo.json�ļ������洢���ֵ�LightSourceInfoDic
        string rootPath = Application.streamingAssetsPath + @"\Json";
        DirectoryInfo root = new DirectoryInfo(rootPath);
        //int pixelCount = 0;
        foreach (var fileInfo in root.GetFiles())
        {
            if (fileInfo.Extension != ".json") continue;
            string[] strArray = fileInfo.Name.Split('^');
            if (strArray.Length <= 1 || strArray[0] != "LightSourceInfo") continue;
            JsonTool.LoadingText(fileInfo.FullName, out string infoText);
            //���ص�����������
            var indexArray = JsonTool.ToObject<LightSourceInfo[]>(infoText);
            for (int i = 0; i < indexArray.Length; i++)
                if (lightSourceInfoDic.ContainsKey(indexArray[i].name) == false)
                {
                    lightSourceInfoDic.Add(indexArray[i].name, indexArray[i]);
                    //indexArray[i].index = pixelCount++;
                    lightSourceInfoList.Add(indexArray[i]);
                }
        }
        #endregion
        //���������������еĹ���

        #region ����GetLine.hlsl
        string autoGetLine = "";
        HashSet<int> radiusHash = new HashSet<int>();
        for (int i = 0; i < lightSourceInfoList.Count; i++)
        {
            int r = lightSourceInfoList[i].radius;
            if (radiusHash.Contains(r) == false)
            {
                radiusHash.Add(r);
                autoGetLine +=
                #region GetLine����
$"#define Line{r} {r + 1}\n" +
$"int GetLine_{r}(int2 pos_G0, int2 pos_G1, out int2 points[Line{r}])\n" +
@"{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;" + "\n" +
$"for (int i = 0; i < Line{r}; i++)\n" +
@"{
points[i] = nowPoint;
int e2 = 2 * err;
if (e2 > -dy)
{
err -= dy;
nowPoint.x += sx;
}
if (e2 < dx)
{
err += dx;
nowPoint.y += sy;
}
}" + "\n" +
$"if (index >= Line{r})\n" +
@"index = -1;
return index;
}" + "\n\n";
                #endregion
            }
        }
        JsonTool.StoreText(Application.dataPath + @"\Resources\PixelRayOcclusion\Auto\" + "GetLine.hlsl", autoGetLine);
        #endregion




        string autoSetLightbuffer =
@"#include ""../Random.hlsl""
#include ""GetLine.hlsl""
#include ""SetLightBuffer_Buffer.hlsl""" + "\n";
        int maxBlcok = Math.Max(Block.Size.x, Block.Size.y);
        for (int i = 0; i < lightSourceInfoList.Count; i++)
        {
            int r = lightSourceInfoList[i].radius;
            int threadX = r * 2 + 1;
            threadX = threadX > maxBlcok ? maxBlcok : threadX;
            autoSetLightbuffer +=
            #region ����SetLightBuffer.hlsl
$"#pragma kernel SetLightBuffer{i}\n" +
$"[numthreads({threadX}, 4, 1)]\n" +
$"void SetLightBuffer{i}(int3 id : SV_DispatchThreadID)\n" +
@"{
    LightSource source = LightSourceBuffer[0];

    int2 gloabPos = IDToGloabPos(id.xy);" + "\n" +
$"    int2 lineArray[Line{r}];\n" +
$"  int sourceIndex = GetLine_{r}(gloabPos, source.gloabPos, lineArray);\n" +
@"    float3 filterColor = float3(source.color.xyz / 255.0);
    int shadow = 1;
    for (int i = sourceIndex; i >= 0; i--)
    {
        if (Equalsi2(GlockToBlock(lineArray[i]), BlockPos))
        {
        int index = PixelToIndex(GloabToPixel(lineArray[i]));" + "\n" +
$"      float weak = pow(clamp(1 - distance(lineArray[i], lineArray[sourceIndex]) / (Line{r} + 1), 0, 1), 2);\n" +
@"      int3 color = filterColor * 255 * weak;
        InterlockedAdd(LightBufferTemp[index].x, color.y);
        InterlockedAdd(LightBufferTemp[index].y, color.y);
        InterlockedAdd(LightBufferTemp[index].z, color.z);
        InterlockedAdd(LightBufferTemp[index].w, 1);
        }
        PixelColorInfo info = GetPixel(lineArray[i]);
        if (info.affectsLight)
        {
            float4 infoColor = info.color / 255.0;
            if (infoColor.w >= 1)
                shadow = 0;
            filterColor.xyz = filterColor.xyz * (1 - infoColor.w) + infoColor.xyz * infoColor.w;
            filterColor.xyz *= shadow;
        }
    }
}" + "\n\n";
            #endregion
        }
        JsonTool.StoreText(Application.dataPath + @"\Resources\PixelRayOcclusion\Auto\" + "SetLightBuffer.compute", autoSetLightbuffer);

        #region ����SetLightBuffer_Buffer.hlsl�ļ�
        JsonTool.LoadingText(Application.streamingAssetsPath + @"\Json\PROconfig.json", out string proConfigText);
        PROconfig proConfig = JsonTool.ToObject<PROconfig>(proConfigText);
        string autoSetLightBuffer_Buffer = "#include \"../StructData.hlsl\"\n";
        int BlockLength = proConfig.EachBlockReceiveLightSize.x * proConfig.EachBlockReceiveLightSize.y;
        for (int i = 0; i < BlockLength; i++)
            autoSetLightBuffer_Buffer += $"StructuredBuffer<int> BlockBuffer{i};\n";
        autoSetLightBuffer_Buffer +=
@"int GetBlockBuffer(int buffer, int index)
{
    switch (buffer)
    {
        default:" + "\n";
        for (int i = 0; i < BlockLength; i++)
            autoSetLightBuffer_Buffer += $"case {i}:\n return BlockBuffer{i}[index];\n";
        autoSetLightBuffer_Buffer +=
        #region ʣ�ಿ��
@"    }
}
RWStructuredBuffer<int4> LightBufferTemp;
//����ɫ����
StructuredBuffer<PixelColorInfo> AllPixelColorInfo;
//��Դ
StructuredBuffer<LightSource> LightSourceBuffer;

//ÿ��������ն��Χ�ڵĹ���
int2 EachBlockReceiveLightSize;
//��ǰ��������ĸ�����Ĺ���
int2 BlockPos;


//���չ������꣬�ڵ�ǰ����Ľ��չ��յķ�Χ�ڣ������½�Ϊԭ��
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

//��������ת���������
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

//���ݱ������귵�ص���Ϣ
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
}";
        #endregion
        JsonTool.StoreText(Application.dataPath + @"/Resources/PixelRayOcclusion/Auto/SetLightBuffer_Buffer.hlsl", autoSetLightBuffer_Buffer);
        #endregion
    }
}