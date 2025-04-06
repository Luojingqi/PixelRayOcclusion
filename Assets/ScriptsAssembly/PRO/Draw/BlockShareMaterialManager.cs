using System;
using UnityEngine;
using static PRO.BlockMaterial;
namespace PRO.Renderer
{
    public class BlockShareMaterialManager
    {
        /// <summary>
        /// background的公共材质
        /// </summary>
        public static Material ShareMaterial { get { return shareMaterial; } }
        private static Material shareMaterial;

        private ComputeBuffer[] blockBufferArray;
        private MaterialPropertyBlock[] materialPropertyBlockArray;
        public void Init()
        {
            blockBufferArray = new ComputeBuffer[BlockBufferLength];
            materialPropertyBlockArray = new MaterialPropertyBlock[LightResultBufferLength];
            unsafe
            {
                for (int i = 0; i < BlockBufferLength; i++)
                    blockBufferArray[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(BlockBase.TextureData.BlockPixelInfo));
            }
            for (int i = 0; i < LightResultBufferLength; i++)
                materialPropertyBlockArray[i] = new MaterialPropertyBlock();


            LoadMaterial();
        }
        private void LoadMaterial()
        {
            shareMaterial = Resources.Load<Material>("PixelRayOcclusion/PRO_Block");
        }

        public void FirstBind()
        {
            shareMaterial.SetBuffer("AllPixelColorInfo", pixelColorInfoToShaderBufffer);

            UpdateBind();
        }

        public void ClearLastBind()
        {
            Vector2Int minLightBufferBlockPos = LastCameraCenterBlockPos - LightResultBufferBlockSize / 2;
            for (int y = 0; y < LightResultBufferBlockSize.y; y++)
                for (int x = 0; x < LightResultBufferBlockSize.x; x++)
                {
                    Vector2Int globalBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    Block block = SceneManager.Inst.NowScene.GetBlock(globalBlockPos);
                    block.spriteRenderer.SetPropertyBlock(NullMaterialPropertyBlock);
                }
        }

        public void UpdateBind()
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;
            Vector2Int maxBlockBufferPos = minBlockBufferPos + LightResultBufferBlockSize - new Vector2Int(1, 1) + EachBlockReceiveLightSize - new Vector2Int(1, 1);

            for (int y = 0; y < LightResultBufferBlockSize.y; y++)
                for (int x = 0; x < LightResultBufferBlockSize.x; x++)
                {
                    Vector2Int globalBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    Vector2Int localBlockBufferPos = globalBlockPos - minBlockBufferPos;
                    int blockIndex = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightResultBufferBlockSize.x);
                    int lightIndex = x + y * LightResultBufferBlockSize.x;
                    Block block = SceneManager.Inst.NowScene.GetBlock(globalBlockPos);
                    //Debug.Log($"块坐标{block.BlockPos}  更新绑定 块缓存索引{blockIndex}  光照缓存索引{lightIndex}");
                    materialPropertyBlockArray[lightIndex].SetBuffer("BlockBuffer", blockBufferArray[blockIndex]);
                    materialPropertyBlockArray[lightIndex].SetBuffer("LightResultBuffer", computeShaderManager.lightResultBufferCSArray[lightIndex].LightResultBuffer);
                    block.spriteRenderer.SetPropertyBlock(materialPropertyBlockArray[lightIndex]);
                }
            for (int y = minBlockBufferPos.y; y <= maxBlockBufferPos.y; y++)
                for (int x = minBlockBufferPos.x; x <= maxBlockBufferPos.x; x++)
                    SetBlock(SceneManager.Inst.NowScene.GetBlock(new Vector2Int(x, y)));
        }

        public void SetBufferData(int index, Array array)
        {
            blockBufferArray[index].SetData(array);
        }
        public ComputeBuffer GetBuffer(int index) => blockBufferArray[index];
    }
}