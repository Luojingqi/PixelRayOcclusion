﻿namespace PRO.SkillEditor
{
    public abstract class SliceBase_Disk
    {
        public string name;
        public int startFrame = -1;
        public int frameLength;
        public bool enable;
        /// <summary>
        /// 触发帧
        /// </summary>
        /// <param name="agent">执行的行动人</param>>
        /// <param name="frame">当前轨道执行的帧</param>
        /// <param name="index">执行此切片内的索引</param>
        public abstract void UpdateFrame(SkillPlayAgent agent, int frame, int index);
    }
}