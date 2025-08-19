using Google.FlatBuffers;

namespace PRO.SkillEditor
{
    public abstract class Slice_DiskBase
    {
        public string name;
        public int startFrame = -1;
        public int frameLength;
        public bool enable = true;
        /// <summary>
        /// 触发帧
        /// </summary>
        /// <param name="agent">执行的行动人</param>>
        public abstract void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData);

        public abstract void EndFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData);

        public abstract class AllowLogicChangeValueBase { }
    }
    public struct FrameData
    {
        /// <summary>
        /// 轨道内的帧索引
        /// </summary>
        public int trackFrame;
        /// <summary>
        /// 切片内的帧索引
        /// </summary>
        public int sliceFrame;

        public SkillVisual_Disk.PlayTrack track;
        /// <summary>
        /// 轨道索引
        /// </summary>
        public int trackIndex;

        public FrameData(int trackFrame, int sliceFrame, SkillVisual_Disk.PlayTrack track, int trackIndex)
        {
            this.trackFrame = trackFrame;
            this.sliceFrame = sliceFrame;
            this.track = track;
            this.trackIndex = trackIndex;
        }
    }
    public struct SliceHash
    {
        public SkillVisual_Disk.PlayTrack track;
        public int trackIndex;
        public int sliceStartFrame;

        public SliceHash(Slice_DiskBase slice, SkillVisual_Disk.PlayTrack track, int trackIndex)
        {
            this.track = track;
            this.trackIndex = trackIndex;
            sliceStartFrame = slice.startFrame;
        }

        public bool Equals(Slice_DiskBase slice, SkillVisual_Disk.PlayTrack track, int trackIndex)
        {
            return
                this.track == track &&
                this.trackIndex == trackIndex &&
                sliceStartFrame == slice.startFrame;
        }

        public Offset<Flat.SliceHashData> ToDisk(FlatBufferBuilder builder)
        {
            Flat.SliceHashData.StartSliceHashData(builder);
            Flat.SliceHashData.AddTrack(builder, (int)track);
            Flat.SliceHashData.AddTrackIndex(builder, trackIndex);
            Flat.SliceHashData.AddSliceStartFrame(builder, sliceStartFrame);
            return Flat.SliceHashData.EndSliceHashData(builder);
        }
    }
    public static class SliceHashEx
    {
        public static SliceHash ToRAM(this Flat.SliceHashData diskData)
        {
            return new SliceHash()
            {
                track = (SkillVisual_Disk.PlayTrack)diskData.Track,
                trackIndex = diskData.TrackIndex,
                sliceStartFrame = diskData.SliceStartFrame
            };
        }
    }
}
