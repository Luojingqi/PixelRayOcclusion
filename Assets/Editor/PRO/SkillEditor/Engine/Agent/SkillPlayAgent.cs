using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.SkillEditor
{
    /// <summary>
    /// 技能播放的执行人
    /// </summary>
    public class SkillPlayAgent : SerializedMonoBehaviour
    {
        #region 对象池
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
        /// 攻击检测轨道每帧返回存储
        /// </summary>
        public Dictionary<string, List<RaycastHit2D>> AttackTestTrack2DDic = new Dictionary<string, List<RaycastHit2D>>();
        /// <summary>
        /// 特效轨道的精灵图渲染器
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();
        [ShowInInspector]
        /// <summary>
        /// 当前播放的技能，设置会立即播放
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

        [LabelText("空闲时播放")]
        public Skill_Disk idle;
        [ShowInInspector]
        /// <summary>
        /// 播放，暂停播放技能
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
