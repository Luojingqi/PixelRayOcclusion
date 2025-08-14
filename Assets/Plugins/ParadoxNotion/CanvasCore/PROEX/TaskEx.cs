using Google.FlatBuffers;
using PRO.BT.Flat;
namespace NodeCanvas.Framework
{
    partial class Task
    {
        protected virtual void ExtendToDisk(FlatBufferBuilder builder) { }
        protected virtual void ExtendToRAM(FlatBufferBuilder builder) { }
    }

    partial class ConditionTask
    {
        public void ToDisk(FlatBufferBuilder builder)
        {
            var extendBuilder = FlatBufferBuilder.TakeOut(1024 * 2);
            ExtendToDisk(extendBuilder);
            var extendBuilderOffset = builder.CreateVector_Builder(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
            ConditionTaskData.StartConditionTaskData(builder);
            ConditionTaskData.AddYieldReturn(builder, yieldReturn);
            ConditionTaskData.AddYields(builder, yields);
            ConditionTaskData.AddIsRuntimeEnabled(builder, isRuntimeEnabled);
            ConditionTaskData.AddExtend(builder, extendBuilderOffset);
            builder.Finish(builder.EndTable());
        }

        public void ToRAM(FlatBufferBuilder builder)
        {
            var diskData = ConditionTaskData.GetRootAsConditionTaskData(builder.DataBuffer);
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

    partial class ActionTask
    {
        public void ToDisk(FlatBufferBuilder builder)
        {
            var extendBuilder = FlatBufferBuilder.TakeOut(1024 * 2);
            ExtendToDisk(extendBuilder);
            var extendBuilderOffset = builder.CreateVector_Builder(extendBuilder);
            FlatBufferBuilder.PutIn(extendBuilder);
            ActionTaskData.StartActionTaskData(builder);
            ActionTaskData.AddStatus(builder, (byte)status);
            ActionTaskData.AddTimeStarted(builder, timeStarted);
            ActionTaskData.AddLatch(builder, latch);
            ActionTaskData.AddExtend(builder, extendBuilderOffset);
            builder.Finish(builder.EndTable());
        }

        public void ToRAM(FlatBufferBuilder builder)
        {
            var diskData = ActionTaskData.GetRootAsActionTaskData(builder.DataBuffer);
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
}
