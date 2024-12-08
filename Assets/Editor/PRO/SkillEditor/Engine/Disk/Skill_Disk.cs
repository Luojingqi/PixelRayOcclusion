using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    /// <summary>
    /// 一个技能存在磁盘中的数据
    /// </summary>
    [CreateAssetMenu(menuName = "创建一个技能配置")]
    public class Skill_Disk : SerializedScriptableObject
    {
        [LabelText("最大帧")]
        public int MaxFrame = 5;
        [LabelText("每帧持续时间/ms")]
        public int FrameTime = 100;

        [LabelText("2D动画轨道")]
        public List<Track_Disk> AnimationTrack2DList = new List<Track_Disk>();
        [LabelText("2D特效轨道")]
        public List<Track_Disk> SpecialEffectTrack2DList = new List<Track_Disk>();
        [LabelText("2D攻击检测轨道")]
        public List<Track_Disk> AttackTestTrack2DList = new List<Track_Disk>();
        [LabelText("场景破坏轨道")]
        public List<Track_Disk> SceneRuinTrackList = new List<Track_Disk>();
        [LabelText("事件轨道")]
        public List<Track_Disk> EventTrackList = new List<Track_Disk>();

        public Skill_Disk()
        {

        }

        public void Clear()
        {
            MaxFrame = 0;
            FrameTime = 200;
            AnimationTrack2DList.Clear();
            AttackTestTrack2DList.Clear();
            EventTrackList.Clear();
        }


        public void UpdateFrame(SkillPlayAgent agent, int frame)
        {
            if (agent == null) return;
            int i = 0;
            foreach (var track in AnimationTrack2DList) UpdateFrame(track, agent, frame, i++);
            i = 0;
            foreach (var track in SpecialEffectTrack2DList) UpdateFrame(track, agent, frame, i++);
            i = 0;
            foreach (var track in AttackTestTrack2DList) UpdateFrame(track, agent, frame, i++);
            i = 0;
            foreach (var track in SceneRuinTrackList) UpdateFrame(track, agent, frame, i++);
            i = 0;
            foreach (var track in EventTrackList) UpdateFrame(track, agent, frame, i++);
            i = 0;

            foreach (var kv in agent.AttackTestTrack2DDic) SkillPlayAgent.ListPool.PutIn(kv.Value);
            agent.AttackTestTrack2DDic.Clear();
        }
        private void UpdateFrame(Track_Disk track, SkillPlayAgent agent, int frame, int trackIndex)
        {
            if (frame >= track.SlickList.Count) return;
            SliceBase_Disk slice = track.SlickList[frame];
            if (slice.enable)
                slice.UpdateFrame(agent, frame, frame - slice.startFrame, trackIndex);
        }
    }
}
