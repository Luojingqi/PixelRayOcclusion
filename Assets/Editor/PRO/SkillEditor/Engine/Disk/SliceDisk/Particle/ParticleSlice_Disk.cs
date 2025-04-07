using UnityEngine;

namespace PRO.SkillEditor
{
    public class ParticleSlice_Disk : Slice_DiskBase
    {
        /// <summary>
        /// ���Ӵ�����λ��
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// ���Ӽ���·��
        /// </summary>
        public string loadPath;
        /// <summary>
        /// ���䷽��min
        /// </summary>
        public Vector3 directionMin;
        /// <summary>
        /// ���䷽��max
        /// </summary>
        public Vector3 directionMax;
        /// <summary>
        /// �������
        /// </summary>
        public Vector2 forceRange = new Vector2(1, 1);
        /// <summary>
        /// ���������
        /// </summary>
        public Vector2Int numRange = new Vector2Int(1, 10);
        /// <summary>
        /// ���ʱ�䷶Χ����λms��
        /// </summary>
        public Vector2Int surviveTimeRange = new Vector2Int(int.MaxValue, int.MaxValue);

        public Color color = Color.white;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            try
            {
                var pool = ParticleManager.Inst.GetPool(loadPath);
                int num = Random.Range(numRange.x, numRange.y + 1);
                for (int i = 0; i < num; i++)
                {
                    Particle particle = pool.TakeOutT();
                    particle.Renderer.color = color;
                    particle.SurviveTimeRange = surviveTimeRange;
                    particle.transform.position = agent.transform.rotation * position + agent.transform.position;
                    particle.Rig2D.AddForce(agent.transform.rotation * Vector3.Lerp(directionMin, directionMax, Random.Range(0f, 1f)) * Random.Range(forceRange.x, forceRange.y), ForceMode2D.Impulse);
                }
            }
            catch
            {
            }
        }
    }
}