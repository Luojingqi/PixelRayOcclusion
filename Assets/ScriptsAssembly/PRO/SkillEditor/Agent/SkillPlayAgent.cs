using Google.FlatBuffers;
using PRO.Tool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.SkillEditor
{
    /// <summary>
    /// ���ܲ��ŵ�ִ����
    /// </summary>
    public class SkillPlayAgent : MonoScriptBase, IScene, ITime_Update
    {
        public SceneEntity Scene => _scene;
        private SceneEntity _scene;
        public void SetScene(SceneEntity scene) => _scene = scene;
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
                ClearTimeAndBuffer();
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


        private float time;
        public void TimeUpdate()
        {
            if (Play == false || skill == null) return;
            if (UpdateFrameScript(Skill))
            {
                Skill = idle;
            }
        }
        #region ��������
        /// <summary>
        /// ������Զ����ţ������ʹ���ֶ�api���ţ���Ҫÿ֡����
        /// ����Ϊ����������
        /// </summary>
        /// <param name="playSkill"></param>
        /// <param name="playTrack"></param>
        /// <param name="autoClear">�Ƿ񲥷���Ϻ��Զ���������ʱ��ͻ���������</param>
        /// <returns></returns>
        public bool UpdateFrameScript(Skill_Disk playSkill, int playTrack = ~0, bool autoClear = true)
        {
            time += TimeManager.deltaTime * 1000;
            while (time >= playSkill.FrameTime)
            {
                time -= playSkill.FrameTime;
                playSkill.UpdateFrame(this, nowFrame++, playTrack);
                if (nowFrame >= playSkill.MaxFrame)
                {
                    if (autoClear) ClearTimeAndBuffer();
                    return true;
                }
            }
            return false;
        }

        public void ClearTimeAndBuffer()
        {
            nowFrame = 0;
            time = 0;
        }
        #endregion

        public Offset<Flat.SkillPlayerAgentData> ToDisk(FlatBufferBuilder builder)
        {
            var idleLoadPathOffset = builder.CreateString(idle?.loadPath); 
            var skillLoadPathOffset = builder.CreateString(skill?.loadPath);

            Flat.SkillPlayerAgentData.StartSkillPlayerAgentData(builder);
            Flat.SkillPlayerAgentData.AddPlay(builder, play);
            Flat.SkillPlayerAgentData.AddTime(builder, time);
            Flat.SkillPlayerAgentData.AddNowFrame(builder,nowFrame);
            Flat.SkillPlayerAgentData.AddIdleLoadPath(builder, idleLoadPathOffset);
            Flat.SkillPlayerAgentData.AddSkillLoadPath(builder, skillLoadPathOffset);
            return Flat.SkillPlayerAgentData.EndSkillPlayerAgentData(builder);
        }
        public void ToRAM(Flat.SkillPlayerAgentData diskData)
        {
            play = diskData.Play;
            time = diskData.Time;
            nowFrame = diskData.NowFrame;
            if(diskData.IdleLoadPath != null)
                idle = AssetManager.Load_A<Skill_Disk>("skill.ab", @$"ScriptsAssembly\PRO\����\{diskData.IdleLoadPath}.asset");
            if(diskData.SkillLoadPath != null)
                skill = AssetManager.Load_A<Skill_Disk>("skill.ab", @$"ScriptsAssembly\PRO\����\{diskData.SkillLoadPath}.asset");
            skill?.UpdateFrame(this, nowFrame, (int)(Skill_Disk.PlayTrack.AnimationTrack2D | Skill_Disk.PlayTrack.SpecialEffectTrack2D));
        }
    }
}
