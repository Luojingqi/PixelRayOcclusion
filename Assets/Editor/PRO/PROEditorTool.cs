using PRO;
using PRO.Tool.Serialize.IO;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PROEditorTool
{

    [MenuItem("PRO/1.启动ExcelTool")]
    public static void StartExcelToolEXE()
    {
        ExcelTool.ExcelMain.Start();
    }
    [MenuItem("PRO/2.创建HLSL文件")]
    public static void AutoCreateHLSL()
    {
        #region 创建GetLine.hlsl
        string autoGetLine = "";
        HashSet<int> radiusHash = new HashSet<int>();
        for (int r = 1; r <= BlockMaterial.LightRadiusMax; r++)
        {
            if (radiusHash.Contains(r) == false)
            {
                radiusHash.Add(r);
                autoGetLine +=
                #region GetLine函数
$"#define Line{r} {r + 1}\n" +
$"int GetLine_{r}(int2 pos_G0, int2 pos_G1, out int2 points[Line{r}])\n" +
@"{
int dx = abs(pos_G1.x - pos_G0.x);
int dy = abs(pos_G1.y - pos_G0.y);
int sx = pos_G0.x < pos_G1.x ? 1 : -1;
int sy = pos_G0.y < pos_G1.y ? 1 : -1;
int err = dx - dy;
int e2;
int Index = (dx > dy) ? dx : dy;
int2 nowPoint = pos_G0;" + "\n" + "[unroll]\n" +
$"for (int i = 0; i < Line{r}; i++)\n" +
@"{
points[i] = nowPoint;
e2 = 2 * err;
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
$"if (Index >= Line{r})\n" +
@"Index = -1;
return Index;
}" + "\n\n";
                #endregion
            }
        }
        IOTool.SaveText(Application.dataPath + @"\Resources\PixelRayOcclusion\Auto\" + "GetLine.hlsl", autoGetLine);
        #endregion




        string autoSetLightbuffer =
@"#include ""../Random.hlsl""
#include ""GetLine.hlsl""
#include ""SetLightBuffer_Buffer.hlsl""" + "\n";
        int maxBlcok = Math.Max(Block.Size.x, Block.Size.y);
        for (int r = 1; r <= BlockMaterial.LightRadiusMax; r++)
        {
            int threadX = r * 2 + 1;
            threadX = threadX > maxBlcok ? maxBlcok : threadX;
            autoSetLightbuffer +=
            #region 创建SetLightResultBuffer.hlsl
$"#pragma kernel SetLightResultBuffer{r}\n" +
$"[numthreads({threadX}, 4, 1)]\n" +
$"void SetLightResultBuffer{r}(int3 id : SV_DispatchThreadID)\n" +
@"{
    LightSource source = LightSourceBuffer[0];

    int2 globalPos = IDToGloabPos(id.xy);" + "\n" +
$"    int2 lineArray[Line{r}];\n" +
$"  int sourceIndex = GetLine_{r}(globalPos, source.globalPos, lineArray);\n" +
@"  float3 sourceColor = float3(source.color.xyz / 255.0);  
    float3 filterColor = sourceColor;
    float shadow = 1;
    float lastAffects = -1;
    for (int i = sourceIndex; i >= 0; i--)
    {
        if (Equalsi2(GlockToBlock(lineArray[i]), BlockPos))
        {
        int Index = PixelToIndex(GlobalToPixel(lineArray[i]));" + "\n" +
@$"     float d = distance(lineArray[i], lineArray[sourceIndex]);
        if(d > {r}) return;
        float k = 0.1;
        float attenuation = pow(({r} - d) / {r} , 1.5) ;" + "\n" +
$"      float weak =  pow(clamp(1 - d / (Line{r} - 1), 0, 1), 1);\n" +
@"      
        int3 color = filterColor * 255 * attenuation * shadow;
        InterlockedAdd(LightResultBufferTemp[Index].x, color.x);
        InterlockedAdd(LightResultBufferTemp[Index].y, color.y);
        InterlockedAdd(LightResultBufferTemp[Index].z, color.z);
        InterlockedAdd(LightResultBufferTemp[Index].w, 1);
        }
        BlockPixelInfo pixelInfo = GetBlockPixelInfo(lineArray[i]);
        PixelColorInfo colorInfo = GetPixelColorInfo(pixelInfo.colorInfoId);
        if(colorInfo.affectsLightIntensity != lastAffects)
        {
            float4 infoColor = colorInfo.color / 255.0;
            filterColor.xyz = min(filterColor.xyz * (1 - colorInfo.lightPathColorMixing) + infoColor.xyz * colorInfo.lightPathColorMixing , filterColor.xyz);
            float affectsLightIntensity = colorInfo.affectsLightIntensity * pow(pixelInfo.durability , 0.75);
            shadow *= (1 - affectsLightIntensity);
            lastAffects = affectsLightIntensity;
        }
    }
}" + "\n\n";
            #endregion
        }
        IOTool.SaveText(Application.dataPath + @"\Resources\PixelRayOcclusion\Auto\" + "SetLightBuffer.compute", autoSetLightbuffer);

        #region 创建SetLightBuffer_Buffer.hlsl文件
        string autoSetLightBuffer_Buffer = "#include \"../StructData.hlsl\"\n";
        int BlockLength = BlockMaterial.EachBlockReceiveLightSize.x * BlockMaterial.EachBlockReceiveLightSize.y;
        for (int i = 0; i < BlockLength; i++)
            autoSetLightBuffer_Buffer += $"StructuredBuffer<BlockPixelInfo> BlockBuffer{i};\n";
        autoSetLightBuffer_Buffer +=
@"BlockPixelInfo GetBlockBuffer(int buffer, int Index)
{
    switch (buffer)
    {
        default:" + "\n";
        for (int i = 0; i < BlockLength; i++)
            autoSetLightBuffer_Buffer += $"case {i}:\n return BlockBuffer{i}[Index];\n";
        autoSetLightBuffer_Buffer +=
        #region 剩余部分
@"    }
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
}";
        #endregion
        IOTool.SaveText(Application.dataPath + @"/Resources/PixelRayOcclusion/Auto/SetLightBuffer_Buffer.hlsl", autoSetLightBuffer_Buffer);
        #endregion

        Debug.Log("生成HLSL完成");
    }


#if 写的一坨屎浪费老子一天时间反复调这个傻逼Proto的api
    private static string[][] protoPath =
    {
        new string[]{@"\ScriptsAssembly\PRO.GenericFramework" , @"\ScriptsAssembly\PRO.GenericFramework\Protobuf\CSharp"},
        new string[]{@"\ScriptsAssembly\PRO" , @"\ScriptsAssembly\PRO\Protobuf\CSharp"},
        new string[]{@"\ScriptsAssembly\PRO" , @"\ScriptsAssembly\PRO\Protobuf\CSharp"},
    };
    [MenuItem("PRO/3.启动Proto.exe")]
    public static void ProtoStart()
    {
        var array = new List<FileInfo>[protoPath.Length];
        for (int i = 0; i < protoPath.Length; i++)
        {
            array[i] = new List<FileInfo>();
            array[i].AddRange(FindProto(new DirectoryInfo(Application.dataPath + protoPath[i][0])));
        }
        StringBuilder bat = new StringBuilder();
        bat.Append("chcp 65001\n");
        bat.Append("@echo off\n");
        bat.Append("protoc");
        for (int i = 0; i < protoPath.Length; i++)
        {
            for (int a = 0; a < array[i].Count; a++)
            {
                var fileInfo = array[i][a];
                foreach (var aa in array)
                    foreach (var f in aa)
                        bat.Append($"\0--proto_path={f.DirectoryName}");
                bat.Append($"\0--csharp_out={Application.dataPath.Replace('/', '\\')}{protoPath[i][1]}");
                bat.Append($"\0{fileInfo.Name}");
                bat.Append('\n');
            }
        }
        bat.AppendLine("pause");

        string batPath = @$"{Application.dataPath}\ScriptsAssembly\PRO.GenericFramework\Protobuf\EXE\toC#.bat";
        File.WriteAllText(batPath, bat.ToString(), Encoding.Default);
        var processInfo = new System.Diagnostics.ProcessStartInfo()
        {
            FileName = batPath,
            Arguments = bat.ToString(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        using (var proto = new System.Diagnostics.Process())
        {
            proto.StartInfo = processInfo;
            proto.Start();
            proto.WaitForExit();
            Debug.Log(proto.StandardError.ReadToEnd() + proto.StandardOutput.ReadToEnd());
        }
    }

    private static List<FileInfo> FindProto(DirectoryInfo nowDirInfo)
    {
        var ret = new List<FileInfo>();
        foreach (var fileInfo in nowDirInfo.GetFiles())
            if (fileInfo.Extension == ".proto")
                ret.Add(fileInfo);
        foreach (var dirInfo in nowDirInfo.GetDirectories())
            ret.AddRange(FindProto(dirInfo));
        return ret;
    }
#endif
}
