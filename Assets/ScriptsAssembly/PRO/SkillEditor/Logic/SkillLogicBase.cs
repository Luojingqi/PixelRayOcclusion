using Google.FlatBuffers;
using PRO.SkillEditor.Flat;
using PROTool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public abstract class SkillLogicBase
    {
        private static Dictionary<string, Type> typeDic;
        private static void Init()
        {
            var typeList = ReflectionTool.GetDerivedClasses(typeof(SkillLogicBase));
            typeDic = new (typeList.Count);
            for (int i = 0; i < typeList.Count; i++)
            {
                var type = typeList[i];
                typeDic.Add(type.Name, type);
            }
        }
        public static SkillLogicBase CreateSkillLogic(string typeName)
        {
            if (typeDic == null) Init();
            return Activator.CreateInstance(typeDic[typeName]) as SkillLogicBase;
        }

        public virtual void Before_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual) { }
        public virtual void Before_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData) { }
        public virtual void Before_Event(SkillPlayAgent agent, SkillPlayData playData, EventDisk_Base slice, FrameData frameData) { }


        public virtual void Agoing_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData, Span<RaycastHit2D> hitSpan) { }
        public virtual void Agoing_创建Building(SkillPlayAgent agent, SkillPlayData playData, EventDisk_创建Building slice, FrameData frameData, BuildingBase building) { }

        public virtual void Update_Event(SkillPlayAgent agent, SkillPlayData playData, EventDisk_Base slice, FrameData frameData, float deltaTime, float time) { }

        public virtual void After_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData) { }
        public virtual void After_Event(SkillPlayAgent agent, SkillPlayData playData, EventDisk_Base slice, FrameData frameData) { }

        public virtual void After_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual) { }

        public Offset<SkillLogicData> ToDisk(FlatBufferBuilder builder)
        {
            var typeOffset = builder.CreateString(GetType().Name);
            var extendDataBuilder = FlatBufferBuilder.TakeOut(1024 * 2);
            ExtendDataToDisk(extendDataBuilder);
            var extendDataOffset = builder.CreateVector_Builder(extendDataBuilder);
            SkillLogicData.StartSkillLogicData(builder);
            SkillLogicData.AddType(builder, typeOffset);
            SkillLogicData.AddData(builder, extendDataOffset);
            return SkillLogicData.EndSkillLogicData(builder);
        }
        public void ToRAM(SkillLogicData diskData)
        {
            var length = diskData.DataLength;
            if (length <= 0) return;
            var extendDataBuilder = FlatBufferBuilder.TakeOut(length);
            var span = extendDataBuilder.DataBuffer.ToSpan(0, length);
            for (int i = length - 1; i >= 0; i--)
                span[length - i - 1] = diskData.Data(i);
            ExtendDataToRAM(extendDataBuilder);
            FlatBufferBuilder.PutIn(extendDataBuilder);
        }

        protected virtual void ExtendDataToDisk(FlatBufferBuilder builder) { }
        protected virtual void ExtendDataToRAM(FlatBufferBuilder builder) { }
    }
}
