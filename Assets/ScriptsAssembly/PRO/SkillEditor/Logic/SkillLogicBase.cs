using Google.FlatBuffers;
using PRO.SkillEditor;
using System;
using UnityEngine;

namespace PRO.Skill
{
    public abstract class SkillLogicBase
    {
        public SkillLogicBase(string guid)
        {
            this.guid = guid;
        }
        public string GUID => guid;
        private string guid;

        public virtual void Before_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual) { }
        public virtual void Before_SpecialEffectSlice2D(SkillPlayAgent agent, SkillPlayData playData, SpecialEffectSlice2D_Disk slice, FrameData frameData) { }
        public virtual void Before_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData) { }
        public virtual void Before_Event(SkillPlayAgent agent, SkillPlayData playData, EventDisk_Base slice, FrameData frameData) { }


        public virtual void Agoing_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData, Span<RaycastHit2D> hitSpan) { }
        public virtual void Agoing_创建Building(SkillPlayAgent agent, SkillPlayData playData, EventDisk_创建Building slice, FrameData frameData, BuildingBase building) { }


        public virtual void After_SpecialEffectSlice2D(SkillPlayAgent agent, SkillPlayData playData, SpecialEffectSlice2D_Disk slice, FrameData frameData) { }
        public virtual void After_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData) { }
        public virtual void After_Event(SkillPlayAgent agent, SkillPlayData playData, EventDisk_Base slice, FrameData frameData) { }

        public virtual void After_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual) { }

        public virtual void ToDisk(FlatBufferBuilder builder) { }
        public virtual void ToRAM(FlatBufferBuilder builder) { }
    }
}
