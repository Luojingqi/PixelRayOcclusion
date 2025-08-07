using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class ParticleSlice_Disk : Slice_DiskBase
    {
        /// <summary>
        /// 粒子创建的位置
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// 粒子加载路径
        /// </summary>
        public string loadPath;
        /// <summary>
        /// 发射方向min
        /// </summary>
        public Vector3 directionMin;
        /// <summary>
        /// 发射方向max
        /// </summary>
        public Vector3 directionMax;
        /// <summary>
        /// 发射的力
        /// </summary>
        public Vector2 forceRange = new Vector2(1, 1);
        /// <summary>
        /// 发射的数量
        /// </summary>
        public Vector2Int numRange = new Vector2Int(1, 10);
        /// <summary>
        /// 存活时间范围（单位ms）
        /// </summary>
        public Vector2Int surviveTimeRange = new Vector2Int(int.MaxValue, int.MaxValue);

        public List<Color32> colorList = new List<Color32>();

        public override void UpdateFrame(SkillPlayAgent agent, SkillVisual_Disk visual, IEnumerable<SkillLogicBase> logics, FrameData frameData)
        {
            try
            {
                var pool = ParticleManager.Inst.GetPool(loadPath);
                int num = Random.Range(numRange.x, numRange.y + 1);
                for (int i = 0; i < num; i++)
                {
                    Particle particle = pool.TakeOut(agent.Scene);
                    if (colorList.Count > 0)
                        particle.Renderer.color = colorList[Random.Range(0, colorList.Count - 1)];
                    particle.SurviveTimeRange = surviveTimeRange;
                    particle.transform.position = agent.transform.rotation * position + agent.transform.position;
                    particle.Rig2D.AddForce(agent.transform.rotation * Vector3.Lerp(directionMin, directionMax, Random.Range(0f, 1f)) * Random.Range(forceRange.x, forceRange.y), ForceMode2D.Impulse);
                }

            }
            catch
            {
            }
        }

        public class AllowLogicChangeValue_ParticleSlice_Disk : AllowLogicChangeValueBase
        {
            public Vector3 position;
            public string loadPath;
            public Vector3 directionMin;
            public Vector3 directionMax;
            public Vector2 forceRange;
            public Vector2Int numRange = new Vector2Int(1, 10);
            public Vector2Int surviveTimeRange = new Vector2Int(int.MaxValue, int.MaxValue);
            public List<Color32> colorList = new List<Color32>();

            public void Reset(ParticleSlice_Disk slice)
            {
                position = slice.position;
                loadPath = slice.loadPath;
                directionMin = slice.directionMin;
                directionMax = slice.directionMax;
                forceRange = slice.forceRange;
                numRange = slice.numRange;
                surviveTimeRange = slice.surviveTimeRange;
                slice.colorList.Clear();
                for (int i = 0; i < slice.colorList.Count; i++)
                    colorList.Add(slice.colorList[i]);
            }
        }
    }
}