using Google.FlatBuffers;
using System.Collections.Generic;
using UnityEngine;
using PRO.SkillEditor.Flat;
using PRO.Flat.Ex;
namespace PRO.SkillEditor
{
    public class EventDisk_Transform : EventDisk_Base
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public bool isPosition = true;
        public bool isRotation = true;
        public bool isScale = true;

        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            if (frameData.sliceFrame == 0)
            {
                var logic = EventDisk_Transform_Logic.TakeOut();
                logic.Start(agent, this, frameData);
                playData.SkillLogicList.Add(logic);
            }

            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Before_Event(agent, playData, this, frameData);
        }

        public override void EndFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].After_Event(agent, playData, this, frameData);
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
            {
                var logic = playData.SkillLogicList[logicIndex];
                if (logic is EventDisk_Transform_Logic transformLogic)
                    if (transformLogic.sliceHash.Equals(this, frameData.trackIndex))
                    {
                        playData.SkillLogicList.RemoveAt(logicIndex);
                        return;
                    }
            }
        }


        private class EventDisk_Transform_Logic : SkillLogicBase
        {
            private static Queue<EventDisk_Transform_Logic> queue = new();
            public static void PutIn(EventDisk_Transform_Logic logic)
            {
                logic.sliceHash = new();
                queue.Enqueue(logic);
            }
            public static EventDisk_Transform_Logic TakeOut()
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
                else
                    return new EventDisk_Transform_Logic();
            }

            public Vector3 startPosition;
            public Vector3 startRotation;
            public Vector3 startScale;

            public Vector3 endPosition;
            public Vector3 endRotation;
            public Vector3 endScale;

            public SliceHash sliceHash;

            public void Start(SkillPlayAgent agent, EventDisk_Transform eventSlice, FrameData frameData)
            {
                sliceHash = new SliceHash(eventSlice, frameData.trackIndex);

                if (eventSlice.isPosition)
                {
                    startPosition = agent.transform.localPosition;
                    endPosition = startPosition + eventSlice.position;
                }
                if (eventSlice.isRotation)
                {
                    startRotation = agent.transform.localRotation.eulerAngles;
                    endRotation = startRotation + eventSlice.rotation;
                }
                if (eventSlice.isScale)
                {
                    startScale = agent.transform.localScale;
                    endScale = startScale + eventSlice.scale;
                }
            }
            public override void Update_Event(SkillPlayAgent agent, SkillPlayData playData, EventDisk_Base slice, FrameData frameData, float deltaTime, float time)
            {
                if (sliceHash.Equals(slice, frameData.trackIndex) == false) return;
                var eventSlice = slice as EventDisk_Transform;
                var tweenTime = time / (slice.frameLength * playData.SkillVisual.FrameTime);
                if (eventSlice.isPosition)
                    agent.transform.localPosition = Vector3.Lerp(startPosition, endPosition, tweenTime);
                if (eventSlice.isRotation)
                    agent.transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, tweenTime));
                if (eventSlice.isScale)
                    agent.transform.localScale = Vector3.Lerp(startScale, endScale, tweenTime);
            }

            protected override void ExtendDataToDisk(FlatBufferBuilder builder)
            {
                var sliceHashOffset = sliceHash.ToDisk(builder);
                EventDisk_Transform_LogicData.StartEventDisk_Transform_LogicData(builder);
                EventDisk_Transform_LogicData.AddStartPosition(builder, startPosition.ToDisk(builder));
                EventDisk_Transform_LogicData.AddStartRotation(builder, startRotation.ToDisk(builder));
                EventDisk_Transform_LogicData.AddStartScale(builder, startScale.ToDisk(builder));
                EventDisk_Transform_LogicData.AddEndPosition(builder, endPosition.ToDisk(builder));
                EventDisk_Transform_LogicData.AddEndRotation(builder, endRotation.ToDisk(builder));
                EventDisk_Transform_LogicData.AddEndScale(builder, endScale.ToDisk(builder));
                EventDisk_Transform_LogicData.AddSliceHash(builder, sliceHashOffset);
                builder.Finish(builder.EndTable());
            }
            protected override void ExtendDataToRAM(FlatBufferBuilder builder)
            {
                var diskData = EventDisk_Transform_LogicData.GetRootAsEventDisk_Transform_LogicData(builder.DataBuffer);
                startPosition = diskData.StartPosition.Value.ToRAM();
                startRotation = diskData.StartRotation.Value.ToRAM();
                startScale = diskData.StartScale.Value.ToRAM();
                endPosition = diskData.EndPosition.Value.ToRAM();
                endRotation = diskData.EndRotation.Value.ToRAM();
                endScale = diskData.EndScale.Value.ToRAM();
                sliceHash = diskData.SliceHash.Value.ToRAM();
            }
        }
    }
}