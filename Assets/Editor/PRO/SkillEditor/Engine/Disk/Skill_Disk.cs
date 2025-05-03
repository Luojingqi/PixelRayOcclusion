using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    /// <summary>
    /// 一个技能存在磁盘中的数据
    /// </summary>
    [CreateAssetMenu(menuName = "创建一个技能轨道")]
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

        public enum PlayTrack : ulong
        {
            /// <summary>
            /// 2D动画轨道
            /// </summary>
            AnimationTrack2D = 1<<0,
            /// <summary>
            /// 2D特效轨道
            /// </summary>
            SpecialEffectTrack2D= 1<<1,
            /// <summary>
            /// 粒子特效轨道
            /// </summary>
            ParticleTrack = 1<<2,
            /// <summary>
            /// 2D攻击检测轨道
            /// </summary>
            AttackTestTrack2D = 1<<3,
            /// <summary>
            /// 场景破坏轨道
            /// </summary>
            SceneRuinTrack = 1<<4,
            /// <summary>
            /// 场景创建轨道
            /// </summary>
            SceneCreateTrack = 1<<5,
            /// <summary>
            /// 事件轨道
            /// </summary>
            EventTrack = 1<<6,
        }
        /// <summary>
        /// 更新某一帧的所有指定轨道的帧切片
        /// </summary>
        /// <param name="agent">执行人</param>
        /// <param name="frame">更新帧</param>
        /// <param name="playTrack">轨道过滤器</param>
        /// <param name="callback">执行完此帧的回调</param>
        public void UpdateFrame(SkillPlayAgent agent, int frame, int playTrack = ~0)
        {
            if (agent == null) return;
            int filter = 1;
            for (int i = 0; i < 7; i++)
            {
                int trackIndex = playTrack & (filter << i);
                int index = 0;
                switch (trackIndex)
                {
                    case (int)PlayTrack.AnimationTrack2D: foreach (var track in AnimationTrack2DList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.SpecialEffectTrack2D: foreach (var track in SpecialEffectTrack2DList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.ParticleTrack: foreach (var track in ParticleTrackList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.AttackTestTrack2D: foreach (var track in AttackTestTrack2DList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.SceneRuinTrack: foreach (var track in SceneRuinTrackList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.SceneCreateTrack: foreach (var track in SceneCreateTrackList) UpdateFrame(track, agent, frame, index++); break;
                    case (int)PlayTrack.EventTrack: foreach (var track in EventTrackList) UpdateFrame(track, agent, frame, index++); break;
                }
            }            
        }
        private void UpdateFrame(Track_Disk track, SkillPlayAgent agent, int frame, int trackIndex)
        {
            if (frame >= track.SlickList.Count) return;
            Slice_DiskBase slice = track.SlickList[frame];
            if (slice.enable)
                slice.UpdateFrame(agent, frame, frame - slice.startFrame, trackIndex);
        }
    }
}
