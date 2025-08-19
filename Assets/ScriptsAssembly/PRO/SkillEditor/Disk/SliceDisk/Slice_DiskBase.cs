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
        /// 轨道的帧索引
        /// </summary>
        public int trackFrame;
        /// <summary>
        /// 切片的帧索引
        /// </summary>
        public int sliceFrame;
        /// <summary>
        /// 轨道索引
        /// </summary>
        public int trackIndex;

        public FrameData(int trackFrame, int sliceFrame, int trackIndex)
        {
            this.trackFrame = trackFrame;
            this.sliceFrame = sliceFrame;
            this.trackIndex = trackIndex;
        }
    }
    public struct SliceHash
    {
        public int trackIndex;
        public string sliceName;
        public int sliceStartFrame;

        public SliceHash(Slice_DiskBase slice, int trackIndex)
        {
            this.trackIndex = trackIndex;
            sliceName = slice.name;
            sliceStartFrame = slice.startFrame;
        }

        public bool Equals(Slice_DiskBase slice, int trackIndex)
        {
            return this.trackIndex == trackIndex &&
                sliceName == slice.name &&
                sliceStartFrame == slice.startFrame;
        }

        public Offset<Flat.SliceHashData> ToDisk(FlatBufferBuilder builder)
        {
            var sliceNameOffset = builder.CreateString(sliceName);
            Flat.SliceHashData.StartSliceHashData(builder);
            Flat.SliceHashData.AddTrackIndex(builder, trackIndex);
            Flat.SliceHashData.AddSliceName(builder, sliceNameOffset);
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
                trackIndex = diskData.TrackIndex,
                sliceName = diskData.SliceName,
                sliceStartFrame = diskData.SliceStartFrame
            };
        }
    }
}
