using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    /// <summary>
    /// 一个技能存在磁盘中的数据
    /// </summary>
    [CreateAssetMenu()]
    public class Skill_Disk : SerializedScriptableObject
    {
        [LabelText("最大帧")]
        public int MaxFrame = 5;
        [LabelText("每帧持续时间/ms")]
        public int FrameTime = 200;

        [LabelText("2D动画轨道")]
        public List<Track_Disk> AnimationTrack2DList = new List<Track_Disk>();
        [LabelText("2D射线检测轨道")]
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
            foreach (var track in AnimationTrack2DList) UpdateFrame(track, agent, frame);
            foreach (var track in AttackTestTrack2DList) UpdateFrame(track, agent, frame);
            foreach (var track in SceneRuinTrackList) UpdateFrame(track, agent, frame);
            foreach (var track in EventTrackList) UpdateFrame(track, agent, frame);


            foreach (var kv in agent.AttackTestTrack2DDic) SkillPlayAgent.ListPool.PutIn(kv.Value);
            agent.AttackTestTrack2DDic.Clear();
        }
        private void UpdateFrame(Track_Disk track, SkillPlayAgent agent, int frame)
        {
            if (frame >= track.SlickList.Count) return;
            SliceBase_Disk slice = track.SlickList[frame];
            if (slice.enable)
                slice.UpdateFrame(agent, frame, frame - slice.startFrame);
        }
    }
}
