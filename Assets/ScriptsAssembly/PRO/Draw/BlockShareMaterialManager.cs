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

        private ComputeBuffer[] blockBuffer;
        public void Init()
        {
            blockBuffer = new ComputeBuffer[BlockBufferLength];
            for (int i = 0; i < BlockBufferLength; i++)
                blockBuffer[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int));

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

        public void UpdateBind()
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;

            for (int y = 0; y < LightBufferBlockSize.y; y++)
                for (int x = 0; x < LightBufferBlockSize.x; x++)
                {
                    Vector2Int gloabBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    Vector2Int localBlockBufferPos = gloabBlockPos - minBlockBufferPos;
                    int blockIndex = localBlockBufferPos.x + localBlockBufferPos.y * (EachBlockReceiveLightSize.x - 1 + LightBufferBlockSize.x);
                    int lightIndex = x + y * LightBufferBlockSize.x;
                    Block block = BlockManager.Inst.BlockCrossList[gloabBlockPos];
                    //Debug.Log($"块坐标{block.BlockPos}  第一次绑定 块缓存索引{blockIndex}  光照缓存索引{lightIndex}");
                    block.materialPropertyBlock.SetBuffer("BlockBuffer", blockBuffer[blockIndex]);
                    block.materialPropertyBlock.SetBuffer("LightBuffer", computeShaderManager.lightBufferCSArray[lightIndex].LightBuffer);
                    block.spriteRenderer.SetPropertyBlock(block.materialPropertyBlock);

                    SetBlock(block);
                }
        }

        public void SetBufferData(int index, Array array)
        {
            blockBuffer[index].SetData(array);
        }
        public ComputeBuffer GetBuffer(int index) => blockBuffer[index];
    }
}