using Google.FlatBuffers;
using PRO.BT.Flat.Base;
using System;
namespace NodeCanvas.BehaviourTrees
{
    partial class BehaviourTree
    {
        public Offset<BehaviourTreeData> ToDisk(FlatBufferBuilder builder)
        {
            Span<int> allNodeOffsetArray = stackalloc int[allNodes.Count];
            for (int i = 0; i < allNodes.Count; i++)
                allNodeOffsetArray[i] = (allNodes[i] as BTNode).ToDisk(builder).Value;
            var allNodeOffsetArrayOffset = builder.CreateVector_Offset(allNodeOffsetArray);

            BehaviourTreeData.StartBehaviourTreeData(builder);
            BehaviourTreeData.AddDeltaTime(builder, deltaTime);
            BehaviourTreeData.AddElapsedTime(builder, elapsedTime);
            BehaviourTreeData.AddRootStatus(builder, (byte)_rootStatus);
            BehaviourTreeData.AddIntervalCounter(builder, intervalCounter);
            BehaviourTreeData.AddAllNode(builder, allNodeOffsetArrayOffset);
            return BehaviourTreeData.EndBehaviourTreeData(builder);
        }

        public void ToRAM(BehaviourTreeData diskData)
        {
            deltaTime = diskData.DeltaTime;
            elapsedTime = diskData.ElapsedTime;
            _rootStatus = (Framework.Status)diskData.RootStatus;
            intervalCounter = diskData.IntervalCounter;
            for (int i = allNodes.Count - 1; i >= 0; i--)
            {
                var nodeDiskData = diskData.AllNode(i).Value;
                var node = allNodes[allNodes.Count - i - 1] as BTNode;
                node.ToRAM(nodeDiskData);
            }
        }
    }

    partial class BTNode
    {
        public Offset<BTNodeData> ToDisk(FlatBufferBuilder buider)
        {
            var extendBuilder = FlatBufferBuilder.TakeOut(1024 * 2);
            ExtendToDisk(extendBuilder);
            var extendOffset = buider.CreateVector_Builder(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);

            BTNodeData.StartBTNodeData(buider);
            BTNodeData.AddStatus(buider, (byte)_status);
            BTNodeData.AddTimeStarted(buider, timeStarted);
            BTNodeData.AddExtend(buider, extendOffset);
            return BTNodeData.EndBTNodeData(buider);
        }
        public void ToRAM(BTNodeData diskData)
        {
            _status = (Framework.Status)diskData.Status;
            timeStarted = diskData.TimeStarted;
            var extendLength = diskData.ExtendLength;
            if (extendLength <= 0) return;
            var extendBuilder = FlatBufferBuilder.TakeOut(extendLength);
            var extendBuilderSpan = extendBuilder.DataBuffer.ToSpan(0, extendLength);
            for (int i = extendBuilderSpan.Length - 1; i >= 0; i--)
                extendBuilderSpan[extendBuilderSpan.Length - i - 1] = diskData.Extend(i);
            ExtendToRAM(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
        }
        protected virtual void ExtendToDisk(FlatBufferBuilder builder) { }
        protected virtual void ExtendToRAM(FlatBufferBuilder builder) { }

    }
}
