using Google.FlatBuffers;
using NodeCanvas.Framework;
using PRO.BT.Flat;
using UnityEngine;
namespace PRO.BT
{
    public class 等待 : ActionTask
    {
        public BBParameter<float> 每次等待倒计时;
        public BBParameter<float> 当前剩余倒计时;
        protected override void OnExecute()
        {
            当前剩余倒计时.value = 每次等待倒计时.value;
        }
        protected override void OnUpdate()
        {
            当前剩余倒计时.value -= TimeManager.deltaTime;
            if (当前剩余倒计时.value <= 0)
            {
                当前剩余倒计时.value = 每次等待倒计时.value;
                EndAction(true); return;
            }
        }

        protected override void ExtendToDisk(FlatBufferBuilder builder)
        {
            WaitData.StartWaitData(builder);
            if (每次等待倒计时.isDefined == false)
                WaitData.AddValue0(builder, 每次等待倒计时.value);
            if (当前剩余倒计时.isDefined == false)
                WaitData.AddValue1(builder, 当前剩余倒计时.value);
            builder.Finish(builder.EndTable());
        }
        protected override void ExtendToRAM(FlatBufferBuilder builder)
        {
            var diskData = WaitData.GetRootAsWaitData(builder.DataBuffer);
            if (每次等待倒计时.isDefined == false)
                每次等待倒计时 = diskData.Value0;
            if (当前剩余倒计时.isDefined == false)
                当前剩余倒计时 = diskData.Value1;
        }
    }
}
