using PRO.DataStructure;
using PRO.Disk;
using System.Collections.Generic;
using UnityEngine;
using static PRO.BlockMaterial;
using static PRO.Renderer.ComputeShaderManager;
namespace PRO.Renderer
{
    public class LightBufferCS
    {
        private ComputeShader SetLightBufferCS;
        private ComputeShader ResetLightBufferCS;


        public ComputeBuffer LightBuffer;
        private ComputeBuffer LightBufferTemp;

        private ComputeBuffer LightSourceBuffer;
        private LightSourceToShader[] lightSourceArray = new LightSourceToShader[1];
        private ComputeBuffer SLBuffer;

        private Vector2Int gloabBlockPos;
        private Vector2Int localBlockBufferPos;

        public LightBufferCS(ComputeShader SetLightBufferShader, ComputeShader ResetLightBufferShader)
        {
            this.SetLightBufferCS = SetLightBufferShader;
            this.ResetLightBufferCS = ResetLightBufferShader;
            InitBuffer();
        }

        private void InitBuffer()
        {
            LightBuffer = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int) * 4);
            LightBufferTemp = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int) * 4);
            unsafe { LightSourceBuffer = new ComputeBuffer(1, sizeof(LightSourceToShader)); }
        }

        public void FirstBind(Vector2Int gloabBlockPos, Vector2Int localBlockBufferPos)
        {
            this.gloabBlockPos = gloabBlockPos;
            this.localBlockBufferPos = localBlockBufferPos;
            ResetLightBufferCS.SetBuffer(0, "LightBuffer", LightBuffer);
            ResetLightBufferCS.SetBuffer(1, "LightBuffer", LightBuffer); ;
            ResetLightBufferCS.SetBuffer(1, "LightBufferTemp", LightBufferTemp);

            SetLightBufferCS.SetInts("EachBlockReceiveLightSize", EachBlockReceiveLightSize.x, EachBlockReceiveLightSize.y);
            for (int i = 0; i < BlockMaterial.LightSourceInfoListCount; i++)
            {
                SetLightBufferCS.SetBuffer(i, "AllPixelColorInfo", pixelColorInfoToShaderBufffer);
                SetLightBufferCS.SetBuffer(i, "LightSourceBuffer", LightSourceBuffer);
                SetLightBufferCS.SetBuffer(i, "LightBufferTemp", LightBufferTemp);
            }
            for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                {
                    int index = ex + ey * EachBlockReceiveLightSize.x;
                    Vector2Int nowLocalBlockBufferPos = localBlockBufferPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                    int eBlcokIndex = nowLocalBlockBufferPos.x + nowLocalBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                    for (int i = 0; i < BlockMaterial.LightSourceInfoListCount; i++)
                        SetLightBufferCS.SetBuffer(i, $"BlockBuffer{index}", blockShareMaterialManager.GetBuffer(eBlcokIndex));
                    //Debug.Log($"¹âÕÕ¼ÆËã{gloabBlockPos}  °ó¶¨¿é»º´æ{index}   {eBlcokIndex}");
                }

            SetLightBufferCS.SetInts("BlockPos", gloabBlockPos.x, gloabBlockPos.y);
        }

        public void UpdateBind(Vector2Int gloabBlockPos, Vector2Int localBlockBufferPos)
        {
            this.gloabBlockPos = gloabBlockPos;
            this.localBlockBufferPos = localBlockBufferPos;
            for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                {
                    int index = ex + ey * EachBlockReceiveLightSize.x;
                    Vector2Int nowLocalBlockBufferPos = localBlockBufferPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                    int eBlcokIndex = nowLocalBlockBufferPos.x + nowLocalBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                    SetLightBufferCS.SetBuffer(0, $"BlockBuffer{index}", blockShareMaterialManager.GetBuffer(eBlcokIndex));
                }
            SetLightBufferCS.SetInts("BlockPos", gloabBlockPos.x, gloabBlockPos.y);
        }

        public void Update()
        {
            ResetLightBufferCS.Dispatch(0, Block.Size.x / 8, Block.Size.y / 8, 1);
            Vector2Int blockMinPos = Block.PixelToGloab(gloabBlockPos, new(0, 0));
            Vector2Int blockMaxPos = blockMinPos + (Vector2Int)Block.Size - new Vector2Int(1, 1);
            for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                {
                    Vector2Int nowGloabBlockBufferPos = gloabBlockPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                    Block block = SceneManager.Inst.NowScene.GetBlock(nowGloabBlockBufferPos);
                    BackgroundBlock background = SceneManager.Inst.NowScene.GetBackground(nowGloabBlockBufferPos);

                    foreach (var value in block.lightSourceDic.Values) DrawLightSource(value.info, new LightSourceToShader(value), blockMinPos, blockMaxPos);
                    foreach (var value in background.lightSourceDic.Values) DrawLightSource(value.info, new LightSourceToShader(value), blockMinPos, blockMaxPos);
                }
        }

        private void DrawLightSource(LightSourceInfo info, LightSourceToShader lightSource, Vector2Int blockMinPos, Vector2Int blockMaxPos)
        {
            int r = info.radius;
            Vector2Int lightMinRadius = lightSource.gloabPos - new Vector2Int(r, r);
            Vector2Int lightMaxRadius = lightSource.gloabPos + new Vector2Int(r, r);

            Vector2Int beMixed_Min = new Vector2Int(Mathf.Max(blockMinPos.x, lightMinRadius.x), Mathf.Max(blockMinPos.y, lightMinRadius.y));
            Vector2Int beMixed_Max = new Vector2Int(Mathf.Min(blockMaxPos.x, lightMaxRadius.x), Mathf.Min(blockMaxPos.y, lightMaxRadius.y));
            if (beMixed_Min.x <= beMixed_Max.x && beMixed_Min.y <= beMixed_Max.y)
            {
                lightSourceArray[0] = lightSource;
                LightSourceBuffer.SetData(lightSourceArray);
                SetLightBufferCS.SetInts("beMixed_Min", beMixed_Min.x, beMixed_Min.y);
                SetLightBufferCS.SetInts("beMixed_Max", beMixed_Max.x, beMixed_Max.y);

                SetLightBufferCS.Dispatch(info.index, 1, 1, 1);
                ResetLightBufferCS.Dispatch(1, Block.Size.x / 8, Block.Size.y / 8, 1);
            }

        }
    }
}