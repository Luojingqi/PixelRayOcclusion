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
}
