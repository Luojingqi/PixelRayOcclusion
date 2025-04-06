using UnityEngine;
using static PRO.BlockMaterial;
using static PRO.Renderer.ComputeShaderManager;
namespace PRO.Renderer
{
    public class LightResultBufferCS
    {
        public ComputeShader SetLightBufferCS;
        private ComputeShader ResetLightBufferCS;


        public ComputeBuffer LightResultBuffer;
        private ComputeBuffer LightResultBufferTemp;
        private ComputeBuffer FreelyLightResultBuffer;

        private ComputeBuffer LightSourceBuffer;
        private LightSourceToShader[] lightSourceArray = new LightSourceToShader[1];
        private ComputeBuffer SLBuffer;

        private Vector2Int globalBlockPos;
        private Vector2Int localBlockBufferPos;

        public LightResultBufferCS(ComputeShader SetLightBufferShader, ComputeShader ResetLightBufferShader)
        {
            this.SetLightBufferCS = SetLightBufferShader;
            this.ResetLightBufferCS = ResetLightBufferShader;
            InitBuffer();
        }

        private void InitBuffer()
        {
            LightResultBuffer = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int) * 4);
            LightResultBufferTemp = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int) * 4);
            FreelyLightResultBuffer = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int) * 4);
            unsafe { LightSourceBuffer = new ComputeBuffer(1, sizeof(LightSourceToShader)); }
        }

        public void FirstBind(Vector2Int globalBlockPos, Vector2Int localBlockBufferPos)
        {
            this.globalBlockPos = globalBlockPos;
            this.localBlockBufferPos = localBlockBufferPos;
            ResetLightBufferCS.SetBuffer(0, "LightResultBuffer", LightResultBuffer);
            ResetLightBufferCS.SetBuffer(1, "LightResultBuffer", LightResultBuffer);
            ResetLightBufferCS.SetBuffer(1, "LightResultBufferTemp", LightResultBufferTemp);
            ResetLightBufferCS.SetBuffer(2, "LightResultBuffer", LightResultBuffer);
            ResetLightBufferCS.SetBuffer(2, "LightResultBufferTemp", LightResultBufferTemp);
            ResetLightBufferCS.SetBuffer(2, "FreelyLightResultBuffer", FreelyLightResultBuffer);
            ResetLightBufferCS.SetBuffer(3, "LightResultBuffer", LightResultBuffer);
            ResetLightBufferCS.SetBuffer(3, "FreelyLightResultBuffer", FreelyLightResultBuffer);
            ResetLightBufferCS.SetBuffer(4, "FreelyLightResultBuffer", FreelyLightResultBuffer);


            SetLightBufferCS.SetInts("EachBlockReceiveLightSize", EachBlockReceiveLightSize.x, EachBlockReceiveLightSize.y);
            for (int i = 0; i < BlockMaterial.LightRadiusMax; i++)
            {
                SetLightBufferCS.SetBuffer(i, "AllPixelColorInfo", pixelColorInfoToShaderBufffer);
                SetLightBufferCS.SetBuffer(i, "LightSourceBuffer", LightSourceBuffer);
                SetLightBufferCS.SetBuffer(i, "LightResultBufferTemp", LightResultBufferTemp);
            }
            for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                {
                    int index = ex + ey * EachBlockReceiveLightSize.x;
                    Vector2Int nowLocalBlockBufferPos = localBlockBufferPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                    int eBlcokIndex = nowLocalBlockBufferPos.x + nowLocalBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightResultBufferBlockSize.x);
                    for (int i = 0; i < BlockMaterial.LightRadiusMax; i++)
                        SetLightBufferCS.SetBuffer(i, $"BlockBuffer{index}", blockShareMaterialManager.GetBuffer(eBlcokIndex));
                    //Debug.Log($"¹âÕÕ¼ÆËã{globalBlockPos}  °ó¶¨¿é»º´æ{index}   {eBlcokIndex}");
                }

            SetLightBufferCS.SetInts("BlockPos", globalBlockPos.x, globalBlockPos.y);
        }

        public void UpdateBind(Vector2Int globalBlockPos, Vector2Int localBlockBufferPos)
        {
            this.globalBlockPos = globalBlockPos;
            this.localBlockBufferPos = localBlockBufferPos;
            for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                {
                    int index = ex + ey * EachBlockReceiveLightSize.x;
                    Vector2Int nowLocalBlockBufferPos = localBlockBufferPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                    int eBlcokIndex = nowLocalBlockBufferPos.x + nowLocalBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightResultBufferBlockSize.x);
                    SetLightBufferCS.SetBuffer(0, $"BlockBuffer{index}", blockShareMaterialManager.GetBuffer(eBlcokIndex));
                }
            SetLightBufferCS.SetInts("BlockPos", globalBlockPos.x, globalBlockPos.y);
        }

        public void UpdateStaticLightSource()
        {
            ResetLightBufferCS.Dispatch(0, Block.Size.x / 8, Block.Size.y / 8, 1);
            Vector2Int blockMinPos = Block.PixelToGlobal(globalBlockPos, new(0, 0));
            Vector2Int blockMaxPos = blockMinPos + (Vector2Int)Block.Size - new Vector2Int(1, 1);
            for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                {
                    Vector2Int nowGloabBlockBufferPos = globalBlockPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                    Block block = SceneManager.Inst.NowScene.GetBlock(nowGloabBlockBufferPos);
                    BackgroundBlock background = SceneManager.Inst.NowScene.GetBackground(nowGloabBlockBufferPos);

                    if (block != null) foreach (var value in block.lightSourceDic.Values) DrawLightSource(value.radius, new LightSourceToShader(value), blockMinPos, blockMaxPos);
                    if (background != null) foreach (var value in background.lightSourceDic.Values) DrawLightSource(value.radius, new LightSourceToShader(value), blockMinPos, blockMaxPos);
                }
        }
            public void UpdateFreelyLightSource()
            {
                ResetLightBufferCS.Dispatch(4, Block.Size.x / 8, Block.Size.y / 8, 1);
                Vector2Int blockMinPos = Block.PixelToGlobal(globalBlockPos, new(0, 0));
                Vector2Int blockMaxPos = blockMinPos + (Vector2Int)Block.Size - new Vector2Int(1, 1);
                for (int ey = 0; ey < EachBlockReceiveLightSize.y; ey++)
                    for (int ex = 0; ex < EachBlockReceiveLightSize.x; ex++)
                    {
                        Vector2Int nowGloabBlockBufferPos = globalBlockPos - EachBlockReceiveLightSize / 2 + new Vector2Int(ex, ey);
                        Block block = SceneManager.Inst.NowScene.GetBlock(nowGloabBlockBufferPos);
                        if (block != null)
                            foreach (var value in block.FreelyLightSourceHash)
                            {
                                LightSourceToShader lightSource = new LightSourceToShader() { gloabPos = value.GloabPos.Value, color = value.color };
                                int r = value.Radius;
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

                                    SetLightBufferCS.Dispatch(value.Radius - 1, 1, 1, 1);

                                    ResetLightBufferCS.Dispatch(2, Block.Size.x / 8, Block.Size.y / 8, 1);
                                }
                            }
                    }
            }


            public void Reset()
            {
                ResetLightBufferCS.Dispatch(4, Block.Size.x / 8, Block.Size.y / 8, 1);
            }

            public void SubtractionFreelyLightResultBuffer()
            {
                ResetLightBufferCS.Dispatch(3, Block.Size.x / 8, Block.Size.y / 8, 1);
            }

            private void DrawLightSource(int radius, LightSourceToShader lightSource, Vector2Int blockMinPos, Vector2Int blockMaxPos)
            {
                Vector2Int lightMinRadius = lightSource.gloabPos - new Vector2Int(radius, radius);
                Vector2Int lightMaxRadius = lightSource.gloabPos + new Vector2Int(radius, radius);

                Vector2Int beMixed_Min = new Vector2Int(Mathf.Max(blockMinPos.x, lightMinRadius.x), Mathf.Max(blockMinPos.y, lightMinRadius.y));
                Vector2Int beMixed_Max = new Vector2Int(Mathf.Min(blockMaxPos.x, lightMaxRadius.x), Mathf.Min(blockMaxPos.y, lightMaxRadius.y));
                if (beMixed_Min.x <= beMixed_Max.x && beMixed_Min.y <= beMixed_Max.y)
                {
                    lightSourceArray[0] = lightSource;
                    LightSourceBuffer.SetData(lightSourceArray);
                    SetLightBufferCS.SetInts("beMixed_Min", beMixed_Min.x, beMixed_Min.y);
                    SetLightBufferCS.SetInts("beMixed_Max", beMixed_Max.x, beMixed_Max.y);

                    SetLightBufferCS.Dispatch(radius - 1, 1, 1, 1);
                    ResetLightBufferCS.Dispatch(1, Block.Size.x / 8, Block.Size.y / 8, 1);
                }

            }
        }
    }