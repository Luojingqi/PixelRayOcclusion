using System;
using UnityEngine;
using UnityEngine.UIElements;
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

        private ComputeBuffer[] backgroundBufferArray;
        private MaterialPropertyBlock[] materialPropertyBlockArray;
        public void Init()
        {
            backgroundBufferArray = new ComputeBuffer[LightResultBufferLength];
            materialPropertyBlockArray = new MaterialPropertyBlock[LightResultBufferLength];
            unsafe
            {
                for (int i = 0; i < LightResultBufferLength; i++)
                    backgroundBufferArray[i] = new ComputeBuffer(Block.Size.x * Block.Size.y, sizeof(BlockBase.TextureData.BlockPixelInfo));
            }
            for (int i = 0; i < LightResultBufferLength; i++)
                materialPropertyBlockArray[i] = new MaterialPropertyBlock();

            shareMaterial = Resources.Load<Material>("PixelRayOcclusion/PRO_Background");

            shareMaterial.SetBuffer("AllPixelColorInfo", pixelColorInfoToShaderBufffer);
        }

        public void ClearLastBind(SceneEntity scene)
        {
            Vector2Int minLightBufferBlockPos = LastCameraCenterBlockPos - LightResultBufferBlockSize / 2;
            for (int y = 0; y < LightResultBufferBlockSize.y; y++)
                for (int x = 0; x < LightResultBufferBlockSize.x; x++)
                {
                    Vector2Int globalBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    BackgroundBlock background = scene.GetBackground(globalBlockPos);
                    background.spriteRenderer.SetPropertyBlock(NullMaterialPropertyBlock);
                }
        }
        public void UpdateBind(SceneEntity scene)
        {
            for (int y = 0; y < LightResultBufferBlockSize.y; y++)
                for (int x = 0; x < LightResultBufferBlockSize.x; x++)
                {
                    Vector2Int globalBlockPos = MinLightBufferBlockPos + new Vector2Int(x, y);
                    int lightIndex = x + y * LightResultBufferBlockSize.x;
                    BackgroundBlock background = scene.GetBackground(globalBlockPos);
                    //Debug.Log($"背景坐标{background.BlockPos}  第一次绑定  背景缓存索引{lightIndex}  光照缓存索引{lightIndex}");
                    materialPropertyBlockArray[lightIndex].SetBuffer("BlockBuffer", backgroundBufferArray[lightIndex]);
                    materialPropertyBlockArray[lightIndex].SetBuffer("LightResultBuffer", computeShaderManager.lightResultBufferCSArray[lightIndex].LightResultBuffer);
                    background.spriteRenderer.SetPropertyBlock(materialPropertyBlockArray[lightIndex]);

                    SetBackgroundBlock(background);
                }
            for (int y = MinBlockBufferPos.y; y <= MaxBlockBufferPos.y; y++)
                for (int x = MinBlockBufferPos.x; x <= MaxBlockBufferPos.x; x++)
                    SetBackgroundBlock(scene.GetBackground(new Vector2Int(x, y)));
        }

        public void SetBufferData(int index, Array array)
        {
            backgroundBufferArray[index].SetData(array);
        }
    }
}
