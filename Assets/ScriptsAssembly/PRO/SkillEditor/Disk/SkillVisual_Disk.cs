using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    /// <summary>
    /// 一个技能存在磁盘中的数据
    /// </summary>
    [CreateAssetMenu(menuName = "创建一个SkillVisual轨道")]
    public class SkillVisual_Disk : SerializedScriptableObject
    {
        [LabelText("最大帧"), ReadOnly]
        public int MaxFrame = 5;
        [LabelText("每帧持续时间/s")]
        public float FrameTime = 0.1f;

        [LabelText("2D动画轨道"), Button(nameof(AddTrack))]
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
        [ReadOnly]
        [LabelText("加载路径  打ab包时会设置")]
        public string loadPath;

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
        [Flags]
        public enum PlayTrack : ulong
        {
            /// <summary>
            /// 2D动画轨道
            /// </summary>
            AnimationTrack2D = 1 << 0,
            /// <summary>
            /// 2D特效轨道
            /// </summary>
            SpecialEffectTrack2D = 1 << 1,
            /// <summary>
            /// 粒子特效轨道
            /// </summary>
            ParticleTrack = 1 << 2,
            /// <summary>
            /// 2D攻击检测轨道
            /// </summary>
            AttackTestTrack2D = 1 << 3,
            /// <summary>
            /// 场景破坏轨道
            /// </summary>
            SceneRuinTrack = 1 << 4,
            /// <summary>
            /// 场景创建轨道
            /// </summary>
            SceneCreateTrack = 1 << 5,
            /// <summary>
            /// 事件轨道
            /// </summary>
            EventTrack = 1 << 6,
        }
        public List<Track_Disk> this[PlayTrack index]
        {
            get
            {
                switch (index)
                {
                    case PlayTrack.AnimationTrack2D: return AnimationTrack2DList;
                    case PlayTrack.SpecialEffectTrack2D: return SpecialEffectTrack2DList;
                    case PlayTrack.ParticleTrack: return ParticleTrackList;
                    case PlayTrack.AttackTestTrack2D: return AttackTestTrack2DList;
                    case PlayTrack.SceneRuinTrack: return SceneRuinTrackList;
                    case PlayTrack.SceneCreateTrack: return SceneCreateTrackList;
                    case PlayTrack.EventTrack: return EventTrackList;
                    default: return null;
                }
            }
        }

        /// <summary>
        /// 更新某一帧的所有指定轨道的帧切片
        /// </summary>
        public void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData)
        {
            if (agent == null) return;
            for (int i = 0; i < 7; i++)
            {
                int trackIndex = playData.playTrack & (1 << i);
                int index = 0;
                foreach (var track in this[(PlayTrack)trackIndex])
                    UpdateTrackFrame(agent, track, playData, index++);
            }
        }
        private void UpdateTrackFrame(SkillPlayAgent agent, Track_Disk track, SkillPlayData playData, int trackIndex)
        {
            Slice_DiskBase slice = track.SlickArray[playData.nowFrame];
            if (slice != null && slice.enable)
                slice.UpdateFrame(agent, playData, new FrameData(playData.nowFrame, playData.nowFrame - slice.startFrame, trackIndex));
        }
        public void UpdateEndFrame(SkillPlayAgent agent, SkillPlayData playData)
        {
            if (agent == null) return;
            for (int i = 0; i < 7; i++)
            {
                int trackIndex = playData.playTrack & (1 << i);
                int index = 0;
                foreach (var track in this[(PlayTrack)trackIndex])
                    UpdateTrackEndFrame(agent, track, playData, index++);
            }
        }
        private void UpdateTrackEndFrame(SkillPlayAgent agent, Track_Disk track, SkillPlayData playData, int trackIndex)
        {
            Slice_DiskBase slice = track.SlickArray[playData.nowFrame];
            var nextFrame = playData.nowFrame + 1;
            if (slice != null && slice.enable && slice.startFrame + slice.frameLength == nextFrame)
                slice.EndFrame(agent, playData, new FrameData(nextFrame, nextFrame - slice.startFrame, trackIndex));
        }
        [Button("设置最大帧")]
        public void SetMaxFrame(int maxFrame)
        {
            if (maxFrame == MaxFrame) return;
            int length = Mathf.Min(maxFrame, MaxFrame);
            for (int i = 0; i < 7; i++)
            {
                int trackIndex = 1 << i;

                foreach (var track in this[(PlayTrack)trackIndex])
                {
                    var oldArray = track.SlickArray;
                    track.SlickArray = new Slice_DiskBase[maxFrame];
                    Array.Copy(oldArray, track.SlickArray, length);
                }
            }
            MaxFrame = maxFrame;
        }
        [Button("添加轨道")]
        public void AddTrack(PlayTrack track)
        {
            this[track].Add(new Track_Disk(MaxFrame));
        }
    }
}
