using System;
using UnityEngine;
using static PRO.BlockMaterial;
namespace PRO.Renderer
{
    public class BackgroundShareMaterialManager
    {
        /// <summary>
        /// block的公共材质
        /// </summary>
        public static Material ShareMaterial { get { return shareMaterial; } }
        private static Material shareMaterial;

        private ComputeBuffer[] backgroundBuffer;
        public void Init()
        {
            backgroundBuffer = new ComputeBuffer[LightBufferLength];
            for (int i = 0; i < LightBufferLength; i++)
                backgroundBuffer[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(int));

            LoadMaterial();
        }
        private void LoadMaterial()
        {
            shareMaterial = Resources.Load<Material>("PixelRayOcclusion/PRO_Background");
        }

        public void FirstBind()
        {
            shareMaterial.SetBuffer("AllPixelColorInfo", pixelColorInfoToShaderBufffer);

            UpdateBind();
        }

        public void UpdateBind()
        {
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;

            for (int y = 0; y < LightBufferBlockSize.y; y++)
                for (int x = 0; x < LightBufferBlockSize.x; x++)
                {
                    Vector2Int gloabBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    int lightIndex = x + y * LightBufferBlockSize.x;
                    BackgroundBlock background = BlockManager.Inst.BackgroundCrossList[gloabBlockPos];
                    //Debug.Log($"背景坐标{background.BlockPos}  第一次绑定  背景缓存索引{lightIndex}  光照缓存索引{lightIndex}");
                    background.materialPropertyBlock.SetBuffer("BackgroundBuffer", backgroundBuffer[lightIndex]);
                    background.materialPropertyBlock.SetBuffer("LightBuffer", computeShaderManager.lightBufferCSArray[lightIndex].LightBuffer);
                    background.spriteRenderer.SetPropertyBlock(background.materialPropertyBlock);

                    SetBackgroundBlock(background);
                }
        }

        public void SetBufferData(int index, Array array)
        {
            backgroundBuffer[index].SetData(array);
        }
    }
}
