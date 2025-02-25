using PRO.Tool;
using Sirenix.OdinInspector;
using System;
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
                    listPool = new ObjectPool<List<RaycastHit2D>>();
                    listPool.PutInEvent += t => t.Clear();
                }
                return listPool;
            }
        }
        #endregion
        public void Init()
        {
            if (AgentSprice == null)
                AgentSprice = transform.GetComponent<SpriteRenderer>();
            Skill = idle;
            Play = true;
        }
        [LabelText("动画轨道精灵渲染器")]
        public SpriteRenderer AgentSprice;
        /// <summary>
        /// 攻击检测轨道每帧返回存储
        /// </summary>
        public Dictionary<string, List<RaycastHit2D>> AttackTestTrack2DDic = new Dictionary<string, List<RaycastHit2D>>();
        /// <summary>
        /// 特效轨道的精灵图渲染器
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();
        [LabelText("正在播放")]
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
                ClearTime();
                if (value != null)
                {
                    Skill.UpdateFrame(this, 0);
                    play = true;
                }
            }
        }
        private Skill_Disk skill;

        [LabelText("空闲时播放")]
        public Skill_Disk idle;
        [ShowInInspector]
        /// <summary>
        /// 播放，暂停播放技能,
        /// </summary>
        public bool Play
        {
            get { return play; }
            set
            {
                play = value;
                if (play == true && skill == null)
                    Skill = idle;

            }
        }
        private bool play = false;

        public int NowFrame
        {
            get { return nowFrame; }
            set
            {
                if (value < 0) return;
                if (Skill != null && value >= Skill.MaxFrame) return;
                nowFrame = value;
            }
        }
        [ShowInInspector]
        private int nowFrame;


        private void Update()
        {
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
                    Skill = idle;
                    return;
                }
            }
        }
        ///// <summary>
        ///// 不自动播放，使用手动api播放
        ///// </summary>
        //public void UpdateFrameScript(int playTrack)
        //{
        //    time0 += Time.deltaTime * 1000;
        //    while (time0 >= Skill.FrameTime)
        //    {
        //        time0 -= Skill.FrameTime;
        //        Skill.UpdateFrame(this, ++nowFrame, playTrack);
        //        if (nowFrame >= Skill.MaxFrame)
        //        {
        //            Skill = idle;
        //            return;
        //        }
        //    }
        //}
        /// <summary>
        /// 不自动播放，使用手动api播放
        /// </summary>
        public bool UpdateFrameScript(Skill_Disk playSkill, int playTrack, Action action = null)
        {
            time += Time.deltaTime * 1000;
            while (time >= playSkill.FrameTime)
            {
                time -= playSkill.FrameTime;
                playSkill.UpdateFrame(this, nowFrame++, playTrack, action);
                if (nowFrame >= playSkill.MaxFrame)
                {
                    ClearTime();
                    return true;
                }
            }
            return false;
        }

        public void ClearTime()
        {
            nowFrame = 0;
            time = 0;
        }
    }
}
