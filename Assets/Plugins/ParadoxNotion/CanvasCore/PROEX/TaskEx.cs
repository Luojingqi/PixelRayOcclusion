using Google.FlatBuffers;
using PRO.BT.Flat.Base;
using System;
namespace NodeCanvas.Framework
{
    partial class Task
    {
        protected virtual void ExtendToDisk(FlatBufferBuilder builder) { }
        protected virtual void ExtendToRAM(FlatBufferBuilder builder) { }
    }

    partial class ConditionTask
    {
        public Offset<ConditionTaskData> ToDisk(FlatBufferBuilder builder)
        {
            var extendBuilder = FlatBufferBuilder.TakeOut(1024 * 2);
            ExtendToDisk(extendBuilder);
            var extendBuilderOffset = builder.CreateVector_Builder(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
            ConditionTaskData.StartConditionTaskData(builder);
            ConditionTaskData.AddIsInitSuccess(builder, _isInitSuccess);
            ConditionTaskData.AddYieldReturn(builder, yieldReturn);
            ConditionTaskData.AddYields(builder, yields);
            ConditionTaskData.AddIsRuntimeEnabled(builder, isRuntimeEnabled);
            ConditionTaskData.AddExtend(builder, extendBuilderOffset);
            return ConditionTaskData.EndConditionTaskData(builder);
        }

        public void ToRAM(ConditionTaskData diskData)
        {
            _isInitSuccess = diskData.IsInitSuccess;
            yieldReturn = diskData.YieldReturn;
            yields = diskData.Yields;
            isRuntimeEnabled = diskData.IsRuntimeEnabled;
            var extendLength = diskData.ExtendLength;
            if (extendLength <= 0) return;
            var extendBuilder = FlatBufferBuilder.TakeOut(extendLength);
            var span = extendBuilder.DataBuffer.ToSpan(0, extendLength);
            for (int i = span.Length - 1; i >= 0; i--)
                span[span.Length - i - 1] = diskData.Extend(i);
            ExtendToRAM(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
        }
    }

    partial class ConditionList
    {
        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            Span<int> conditionOffsetArray = stackalloc int[conditions.Count];
            var conditionBuilder = FlatBufferBuilder.TakeOut(1024 * 4);
            for (int i = 0; i < conditions.Count; i++)
                conditionOffsetArray[i] = conditions[i].ToDisk(builder).Value;
            var conditionOffsetArrayOffset = builder.CreateVector_Offset(conditionOffsetArray);

            ConditionTaskListData.StartConditionTaskListData(builder);
            ConditionTaskListData.AddConditions(builder, conditionOffsetArrayOffset);
            builder.Finish(builder.EndTable());
        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = ConditionTaskListData.GetRootAsConditionTaskListData(builder.DataBuffer);
            for (int i = conditions.Count - 1; i >= 0; i--)
                conditions[conditions.Count - i - 1].ToRAM(diskData.Conditions(i).Value);
        }
    }

    partial class ActionTask
    {
        public Offset<ActionTaskData> ToDisk(FlatBufferBuilder builder)
        {
            var extendBuilder = FlatBufferBuilder.TakeOut(1024 * 2);
            ExtendToDisk(extendBuilder);
            var extendBuilderOffset = builder.CreateVector_Builder(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
            ActionTaskData.StartActionTaskData(builder);
            ActionTaskData.AddIsInitSuccess(builder, _isInitSuccess);
            ActionTaskData.AddStatus(builder, (byte)status);
            ActionTaskData.AddTimeStarted(builder, timeStarted);
            ActionTaskData.AddLatch(builder, latch);
            ActionTaskData.AddExtend(builder, extendBuilderOffset);
            return ActionTaskData.EndActionTaskData(builder);
        }

        public void ToRAM(ActionTaskData diskData)
        {
            _isInitSuccess = diskData.IsInitSuccess;
            status = (Status)diskData.Status;
            timeStarted = diskData.TimeStarted;
            latch = diskData.Latch;
            var extendLength = diskData.ExtendLength;
            if (extendLength <= 0) return;
            var extendBuilder = FlatBufferBuilder.TakeOut(extendLength);
            var span = extendBuilder.DataBuffer.ToSpan(0, extendLength);
            for (int i = span.Length - 1; i >= 0; i--)
                span[span.Length - i - 1] = diskData.Extend(i);
            ExtendToRAM(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
        }
    }

    partial class ActionList
    {
        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            var finishedIndecesOffset = builder.CreateVector_Data(finishedIndeces.AsSpan());
            Span<int> actionOffsetArray = stackalloc int[actions.Count];
            for (int i = 0; i < actions.Count; i++)
                actionOffsetArray[i] = actions[i].ToDisk(builder).Value;
            var actionOffsetArrayOffset = builder.CreateVector_Offset(actionOffsetArray);

            ActionTaskListData.StartActionTaskListData(builder);
            ActionTaskListData.AddCurrentActionIndex(builder, currentActionIndex);
            ActionTaskListData.AddFinishedIndeces(builder, finishedIndecesOffset);
            ActionTaskListData.AddActions(builder, actionOffsetArrayOffset);
            builder.Finish(builder.EndTable());

        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = ActionTaskListData.GetRootAsActionTaskListData(builder.DataBuffer);
            currentActionIndex = diskData.CurrentActionIndex;
            if (_isInitSuccess)
            {
                finishedIndeces = new bool[diskData.FinishedIndecesLength];
                for (int i = finishedIndeces.Length - 1; i >= 0; i--)
                    finishedIndeces[finishedIndeces.Length - i - 1] = diskData.FinishedIndeces(i);
            }
            for (int i = actions.Count - 1; i >= 0; i--)
                actions[actions.Count - i - 1].ToRAM(diskData.Actions(i).Value);
        }
    }
}
