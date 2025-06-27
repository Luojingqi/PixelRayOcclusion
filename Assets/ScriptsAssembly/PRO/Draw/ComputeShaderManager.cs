using UnityEngine;
using static PRO.BlockMaterial;
namespace PRO.Renderer
{
    public class ComputeShaderManager
    {
        public LightResultBufferCS[] lightResultBufferCSArray;

        public void Init()
        {
            LoadComputeShader();
        }
        private void LoadComputeShader()
        {
            lightResultBufferCSArray = new LightResultBufferCS[LightResultBufferLength];
            ComputeShader setLightBufferCS = Resources.Load<ComputeShader>("PixelRayOcclusion/Auto/SetLightBuffer");
            ComputeShader resetLightBufferCS = Resources.Load<ComputeShader>("PixelRayOcclusion/ResetLightBuffer");
            lightResultBufferCSArray[0] = new LightResultBufferCS(setLightBufferCS, resetLightBufferCS);
            for (int i = 1; i < LightResultBufferLength; i++)
            {
                lightResultBufferCSArray[i] = new LightResultBufferCS(
                        GameObject.Instantiate<ComputeShader>(setLightBufferCS),
                        GameObject.Instantiate<ComputeShader>(resetLightBufferCS));
            }
        }

        public void FirstBind()
        {
            //光缓存块里左下角的坐标
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightResultBufferBlockSize / 2;
            //所有提交到cpu的区块里左下角的
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;

            for (int y = 0; y < LightResultBufferBlockSize.y; y++)
                for (int x = 0; x < LightResultBufferBlockSize.x; x++)
                {
                    Vector2Int globalBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    //指的是提交到cpu的区块里的本地坐标，从左下角开始算起
                    Vector2Int localBlockBufferPos = globalBlockPos - minBlockBufferPos;
                    int lightIndex = x + y * LightResultBufferBlockSize.x;

                    lightResultBufferCSArray[lightIndex].FirstBind(globalBlockPos, localBlockBufferPos);
                }
        }
        public void UpdateBind()
        {
            for (int y = 0; y < LightResultBufferBlockSize.y; y++)
                for (int x = 0; x < LightResultBufferBlockSize.x; x++)
                {
                    Vector2Int globalBlockPos = MinLightBufferBlockPos + new Vector2Int(x, y);
                    Vector2Int localBlockBufferPos = globalBlockPos - MinBlockBufferPos;
                    int lightIndex = x + y * LightResultBufferBlockSize.x;

                    lightResultBufferCSArray[lightIndex].UpdateBind(globalBlockPos, localBlockBufferPos);
                }

            for (int i = 0; i < LightResultBufferLength; i++)
            {
                lightResultBufferCSArray[i].UpdateStaticLightSource();
                lightResultBufferCSArray[i].UpdateFreelyLightSource();
            }
        }
        
        private int offset = 0;
        public void Update()
        {
            for (int i = 0; i < LightResultBufferLength; i++)
            {
                int index = (offset + i) % LightResultBufferLength;
                if (i < FrameUpdateBlockNum)
                {
                    lightResultBufferCSArray[index].UpdateStaticLightSource();
                    lightResultBufferCSArray[index].UpdateFreelyLightSource();
                }
                else
                {
                    lightResultBufferCSArray[index].SubtractionFreelyLightResultBuffer();
                    lightResultBufferCSArray[index].UpdateFreelyLightSource();
                }
            }
            offset = (offset + FrameUpdateBlockNum) % LightResultBufferLength;
        }

        public struct LightSourceToShader
        {
            public Vector2Int gloabPos;
            public Vector3Int color;
            public LightSourceToShader(LightSource lightSource)
            {
                this.gloabPos = lightSource.gloabPos;
                this.color = lightSource.color;
            }
        }
        public struct LightSource
        {
            public Vector2Int gloabPos;
            public int radius;
            public Vector3Int color;

            public LightSource(Vector2Int gloabPos, Color32 color, int radius)
            {
                this.gloabPos = gloabPos;
                this.radius = radius;
                this.color = new Vector3Int(color.r, color.g, color.b);
            }
        }
    }
}