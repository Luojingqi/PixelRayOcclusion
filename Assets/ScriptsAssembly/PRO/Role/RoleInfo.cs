using Google.FlatBuffers;
using System;

namespace PRO
{
    public class RoleInfo
    {
        public class Data<T> where T : struct
        {
            private T value;
            public T Value
            {
                get { return value; }
                set
                {
                    if (SetCheck != null) this.value = SetCheck.Invoke(info, value);
                    else this.value = value;
                    ValueChange?.Invoke(info);

                }
            }
            public Action<RoleInfo> ValueChange;
            public Func<RoleInfo, T, T> SetCheck;
            private RoleInfo info;
            public Data(RoleInfo roleInfo)
            {
                info = roleInfo;
            }
        }

        public Data<int> 最大血量;//1
        public Data<int> 血量;//2
        public Data<int> 临时护甲;//3
        public Data<int>[] 抗性Array = new Data<int>[(int)属性.end];//4
        public Data<float> 闪避率;//5
        public Data<float> 命中率;//6
        public Data<float> 暴击率;//7
        public Data<float> 暴击效果;//8
        public Data<int> 行动点上限;//9
        public Data<int> 行动点恢复;//10
        public Data<int> 行动点初始;//11
        public Data<int> 行动点;//12

        public RoleInfo()
        {
            最大血量 = new Data<int>(this) { Value = 10 };
            血量 = new Data<int>(this) { Value = 10 };
            临时护甲 = new Data<int>(this) { Value = 5 };
            for (int i = 0; i < (int)属性.end; i++)
                抗性Array[i] = new Data<int>(this) { Value = 0 };
            闪避率 = new Data<float>(this) { Value = 0.05f };
            命中率 = new Data<float>(this) { Value = 0.75f };
            暴击率 = new Data<float>(this) { Value = 0.1f };
            暴击效果 = new Data<float>(this) { Value = 1.4f };
            行动点上限 = new Data<int>(this) { Value = 7 };
            行动点恢复 = new Data<int>(this) { Value = 5 };
            行动点初始 = new Data<int>(this) { Value = 6 };
            行动点 = new Data<int>(this) { Value = 0 };
            行动点.SetCheck = (info, value) =>
            {
                if (value > info.行动点上限.Value) return info.行动点上限.Value;
                else return value;
            };
        }

        public void ClearAction()
        {
            最大血量.ValueChange = null;
            血量.ValueChange = null;
            临时护甲.ValueChange = null;
            foreach (var item in 抗性Array)
                item.ValueChange = null;
            闪避率.ValueChange = null;
            命中率.ValueChange = null;
            暴击率.ValueChange = null;
            暴击效果.ValueChange = null;
            行动点上限.ValueChange = null;
            行动点恢复.ValueChange = null;
            行动点初始.ValueChange = null;
            行动点.ValueChange = null;
        }

        public static void Clone(RoleInfo form, RoleInfo to)
        {
            to.最大血量.Value = form.最大血量.Value;
            to.血量.Value = form.血量.Value;
            to.临时护甲.Value = form.临时护甲.Value;
            for (int i = 0; i < form.抗性Array.Length; i++)
                to.抗性Array[i].Value = form.抗性Array[i].Value;
            to.闪避率.Value = form.闪避率.Value;
            to.命中率.Value = form.命中率.Value;
            to.暴击率.Value = form.暴击率.Value;
            to.行动点上限.Value = form.行动点上限.Value;
            to.行动点恢复.Value = form.行动点恢复.Value;
            to.行动点初始.Value = form.行动点初始.Value;
            to.行动点.Value = form.行动点.Value;
        }

        public Offset<Flat.RoleInfoData> ToDisk(FlatBufferBuilder builder)
        {
            Span<int> 抗性Array = stackalloc int[this.抗性Array.Length];
            for (int i = 0; i < this.抗性Array.Length; i++)
                抗性Array[i] = this.抗性Array[i].Value;
            var 抗性ArrayOffset = builder.CreateVector_Data(抗性Array);

            Flat.RoleInfoData.StartRoleInfoData(builder);
            Flat.RoleInfoData.AddValue1(builder, 最大血量.Value);
            Flat.RoleInfoData.AddValue2(builder, 血量.Value);
            Flat.RoleInfoData.AddValue3(builder, 临时护甲.Value);
            Flat.RoleInfoData.AddValue4(builder, 抗性ArrayOffset);
            Flat.RoleInfoData.AddValue5(builder, 闪避率.Value);
            Flat.RoleInfoData.AddValue6(builder, 命中率.Value);
            Flat.RoleInfoData.AddValue7(builder, 暴击率.Value);
            Flat.RoleInfoData.AddValue8(builder, 暴击效果.Value);
            Flat.RoleInfoData.AddValue9(builder, 行动点上限.Value);
            Flat.RoleInfoData.AddValue10(builder, 行动点恢复.Value);
            Flat.RoleInfoData.AddValue11(builder, 行动点初始.Value);
            Flat.RoleInfoData.AddValue12(builder, 行动点.Value);
            return Flat.RoleInfoData.EndRoleInfoData(builder);
        }

        public void ToRAM(Flat.RoleInfoData diskData)
        {
            最大血量.Value = diskData.Value1;
            血量.Value = diskData.Value2;
            临时护甲.Value = diskData.Value3;
            for (int i = 0; i < 抗性Array.Length; i++)
                抗性Array[i].Value = diskData.Value4(抗性Array.Length - i - 1);
            闪避率.Value = diskData.Value5;
            命中率.Value = diskData.Value6;
            暴击率.Value = diskData.Value7;
            暴击效果.Value = diskData.Value8;
            行动点上限.Value = diskData.Value9;
            行动点恢复.Value = diskData.Value10;
            行动点初始.Value = diskData.Value11;
            行动点.Value = diskData.Value12;
        }
    }

    public enum Toward
    {
        right,
        left
    }
}
