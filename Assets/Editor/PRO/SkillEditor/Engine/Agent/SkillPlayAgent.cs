using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.SkillEditor
{
    /// <summary>
    /// ���ܲ��ŵ�ִ����
    /// </summary>
    public class SkillPlayAgent : MonoBehaviour
    {
        #region �����
        private static ObjectPool<List<RaycastHit2D>> listPool;
        public static ObjectPool<List<RaycastHit2D>> ListPool
        {
            get
            {
                if (listPool == null)
                {
                    listPool = new ObjectPool<List<RaycastHit2D>>(100, true);
                    listPool.PutInEvent += t => t.Clear();
                }
                return listPool;
            }
        }
        #endregion

        public void Start()
        {
            AgentSprice = transform.GetComponent<SpriteRenderer>();
        }
        [ShowInInspector]
        public SpriteRenderer AgentSprice { get; protected set; }

        public Dictionary<string, List<RaycastHit2D>> AttackTestTrack2DDic = new Dictionary<string, List<RaycastHit2D>>();
        public Skill_Disk Skill
        {
            get { return skill; }
            set
            {
                skill = value;
                nowFrame = 0;
                skillPlay = false;
            }
        }
        private Skill_Disk skill;


        /// <summary>
        /// ���ţ���ͣ���ż���
        /// </summary>
        public bool SkillPlay
        {
            get { return skillPlay; }
            set
            {
                if (skillPlay == value) return;
                skillPlay = value;
                if (skillPlay == true)
                {
                    Skill.UpdateFrame(this, nowFrame++);
                }
            }
        }
        private bool skillPlay = false;

        public int NowFrame
        {
            get { return nowFrame; }
            set
            {
                if (value < 0 || value >= Skill.MaxFrame) return;
                nowFrame = value;
            }
        }
        private int nowFrame;

        
        public void Update()
        {
            UpdateFrame();
            
        }
        private float time;
        private void UpdateFrame()
        {
            if (SkillPlay == false) return;
            time += Time.deltaTime * 1000;
            while (time >= Skill.FrameTime)
            {
                time -= Skill.FrameTime;
                Skill.UpdateFrame(this, nowFrame++);
                if (nowFrame >= Skill.MaxFrame)
                {
                    skillPlay = false;
                    return;
                }
            }
        }
    }
}
