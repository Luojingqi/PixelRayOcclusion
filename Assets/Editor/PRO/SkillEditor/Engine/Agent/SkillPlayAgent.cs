using PRO.Tool;
using Sirenix.OdinInspector;
using System;
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
        [LabelText("�������������Ⱦ��")]
        public SpriteRenderer AgentSprice;
        /// <summary>
        /// ���������ÿ֡���ش洢
        /// </summary>
        public Dictionary<string, List<RaycastHit2D>> AttackTestTrack2DDic = new Dictionary<string, List<RaycastHit2D>>();
        /// <summary>
        /// ��Ч����ľ���ͼ��Ⱦ��
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();
        [LabelText("���ڲ���")]
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
                ClearTime();
                if (value != null)
                {
                    Skill.UpdateFrame(this, 0);
                    play = true;
                }
            }
        }
        private Skill_Disk skill;

        [LabelText("����ʱ����")]
        public Skill_Disk idle;
        [ShowInInspector]
        /// <summary>
        /// ���ţ���ͣ���ż���,
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
        ///// ���Զ����ţ�ʹ���ֶ�api����
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
        /// ���Զ����ţ�ʹ���ֶ�api����
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
