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
        /// <param name="frame">当前轨道执行的帧</param>
        /// <param name="frameIndex">执行此切片内的索引</param>
        public abstract void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex);

        /// <summary>
        /// 缓冲对象，用于切片之间的数据交互
        /// </summary>
        public interface ISliceBufferData
        {            
            public void PutIn();
        }
    }
}
