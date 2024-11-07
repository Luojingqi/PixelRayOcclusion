using PRO.Disk;
using UnityEngine;
using static PRO.BlockMaterial;
namespace PRO.Renderer
{
    public class ComputeShaderManager
    {
        public LightBufferCS[] lightBufferCSArray;

        public void Init()
        {
            LoadComputeShader();
        }
        private void LoadComputeShader()
        {
            lightBufferCSArray = new LightBufferCS[LightBufferLength];
            ComputeShader setLightBufferCS = Resources.Load<ComputeShader>("PixelRayOcclusion/Auto/SetLightBuffer");
            ComputeShader resetLightBufferCS = Resources.Load<ComputeShader>("PixelRayOcclusion/ResetLightBuffer");
            lightBufferCSArray[0] = new LightBufferCS(setLightBufferCS, resetLightBufferCS);
            for (int i = 1; i < LightBufferLength; i++)
            {
                lightBufferCSArray[i] = new LightBufferCS(
                        GameObject.Instantiate<ComputeShader>(setLightBufferCS),
                        GameObject.Instantiate<ComputeShader>(resetLightBufferCS));
            }
        }

        public void FirstBind()
        {
            //光缓存块里左下角的坐标
            Vector2Int minLightBufferBlockPos = CameraCenterBlockPos - LightBufferBlockSize / 2;
            //所有提交到cpu的区块里左下角的
            Vector2Int minBlockBufferPos = minLightBufferBlockPos - EachBlockReceiveLightSize / 2;

            for (int y = 0; y < LightBufferBlockSize.y; y++)
                for (int x = 0; x < LightBufferBlockSize.x; x++)
                {
                    Vector2Int gloabBlockPos = minLightBufferBlockPos + new Vector2Int(x, y);
                    //指的是提交到cpu的区块里的本地坐标，总左下角开始算起
                    Vector2Int localBlockBufferPos = gloabBlockPos - minBlockBufferPos;
                    int lightIndex = x + y * LightBufferBlockSize.x;

                    lightBufferCSArray[lightIndex].FirstBind(gloabBlockPos, localBlockBufferPos);
                }
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
                    int lightIndex = x + y * LightBufferBlockSize.x;

                    lightBufferCSArray[lightIndex].UpdateBind(gloabBlockPos, localBlockBufferPos);
                }
        }
        private int nowRenderCS = 0;
        public void Update()
        {
            lightBufferCSArray[nowRenderCS++].Update();
            if (nowRenderCS >= LightBufferLength) nowRenderCS = 0;
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
            public LightSourceInfo info;
            public Vector3Int color;

            public LightSource(Vector2Int gloabPos, Color32 color, LightSourceInfo info)
            {
                this.gloabPos = gloabPos;
                this.info = info;
                this.color = new Vector3Int(color.r, color.g, color.b);
            }
        }

        public struct SelfLuminous
        {
            public Vector2Int pos;
            public float strength;
        }
    }
}