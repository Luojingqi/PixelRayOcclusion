using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.SkillEditor
{
    /// <summary>
    /// ���ܲ��ŵ�ִ����
    /// </summary>
    public class SkillPlayAgent : SerializedMonoBehaviour
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

        public void Awake()
        {
            AgentSprice = transform.GetComponent<SpriteRenderer>();
            Skill = idle;
            Play = true;
        }

        public SpriteRenderer AgentSprice;
        /// <summary>
        /// ���������ÿ֡���ش洢
        /// </summary>
        public Dictionary<string, List<RaycastHit2D>> AttackTestTrack2DDic = new Dictionary<string, List<RaycastHit2D>>();
        /// <summary>
        /// ��Ч����ľ���ͼ��Ⱦ��
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();
        [ShowInInspector]
        /// <summary>
        /// ��ǰ���ŵļ��ܣ����û���������
        /// </summary>
        public Skill_Disk Skill
        {
            get { return skill; }
            set
            {
                skill = value;
                nowFrame = 0;
                Skill.UpdateFrame(this, 0);
                play = true;
                time = 0;
            }
        }
        private Skill_Disk skill;

        [LabelText("����ʱ����")]
        public Skill_Disk idle;
        [ShowInInspector]
        /// <summary>
        /// ���ţ���ͣ���ż���
        /// </summary>
        public bool Play
        {
            get { return play; }
            set
            {
                if (play == value) return;
                play = value;
                if (play == false)
                {
                    Skill = idle;
                }
            }
        }
        private bool play = false;

        public int NowFrame
        {
            get { return nowFrame; }
            set
            {
                if (value < 0 || value >= Skill.MaxFrame) return;
                nowFrame = value;
            }
        }
        [ShowInInspector]
        private int nowFrame;


        public void Update()
        {
         //   if (Input.GetKeyDown(KeyCode.V))
                UpdateFrame();
        }
        private float time;
        private void UpdateFrame()
        {
            if (Play == false || skill == null) return;
            time += Time.deltaTime * 1000;
            while (time >= Skill.FrameTime)
            {
                time -= Skill.FrameTime;
                Skill.UpdateFrame(this, ++nowFrame);
                if (nowFrame >= Skill.MaxFrame)
                {
                    Play = false;
                    return;
                }
            }
        }
    }
}
