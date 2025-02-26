using Sirenix.OdinInspector;
using System;
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
        [LabelText("粒子特效轨道")]
        public List<Track_Disk> ParticleTrackList = new List<Track_Disk>();
        [LabelText("2D攻击检测轨道")]
        public List<Track_Disk> AttackTestTrack2DList = new List<Track_Disk>();
        [LabelText("场景破坏轨道")]
        public List<Track_Disk> SceneRuinTrackList = new List<Track_Disk>();
        [LabelText("场景创建轨道")]
        public List<Track_Disk> SceneCreateTrackList = new List<Track_Disk>();
        [LabelText("事件轨道")]
        public List<Track_Disk> EventTrackList = new List<Track_Disk>();

        public void Clear()
        {
            MaxFrame = 0;
            FrameTime = 200;
            AnimationTrack2DList.Clear();
            SpecialEffectTrack2DList.Clear();
            ParticleTrackList.Clear();
            AttackTestTrack2DList.Clear();
            SceneRuinTrackList.Clear();
            EventTrackList.Clear();
        }
        /// <summary>
        /// 改变枚举内部顺序，调用顺序也将改变
        /// </summary>
        public enum PlayTrack
        {
            /// <summary>
            /// 2D动画轨道
            /// </summary>
            AnimationTrack2D,
            /// <summary>
            /// 2D特效轨道
            /// </summary>
            SpecialEffectTrack2D,
            /// <summary>
            /// 粒子特效轨道
            /// </summary>
            ParticleTrack,
            /// <summary>
            /// 2D攻击检测轨道
            /// </summary>
            AttackTestTrack2D,
            /// <summary>
            /// 场景破坏轨道
            /// </summary>
            SceneRuinTrack,
            /// <summary>
            /// 场景创建轨道
            /// </summary>
            SceneCreateTrack,
            /// <summary>
            /// 事件轨道
            /// </summary>
            EventTrack,
            end
        }

        public void UpdateFrame(SkillPlayAgent agent, int frame, int playTrack = ~0, Action callback = null)
        {
            if (agent == null) return;
            int filter = 1;
            for (int i = 0; i < (int)PlayTrack.end; i++)
            {
                int trackIndex = (playTrack & (filter << i)) >> i;
                if (trackIndex != 1) continue;
                int index = 0;
                switch (i)
                {
                    case (int)PlayTrack.AnimationTrack2D: foreach (var track in AnimationTrack2DList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.SpecialEffectTrack2D: foreach (var track in SpecialEffectTrack2DList) UpdateFrame(track, agent, frame, i++); break;
                    case (int)PlayTrack.ParticleTrack: foreach (var track in ParticleTrackList) UpdateFrame(track, agent, frame, i++); break;
                    case (int)PlayTrack.AttackTestTrack2D: foreach (var track in AttackTestTrack2DList) UpdateFrame(track, agent, frame, i++); break;
                    case (int)PlayTrack.SceneRuinTrack: foreach (var track in SceneRuinTrackList) UpdateFrame(track, agent, frame, i++); break;
                    case (int)PlayTrack.SceneCreateTrack: foreach (var track in SceneCreateTrackList) UpdateFrame(track, agent, frame, i++); break;
                    case (int)PlayTrack.EventTrack: foreach (var track in EventTrackList) UpdateFrame(track, agent, frame, i++); break;
                }
            }
            callback?.Invoke();
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
