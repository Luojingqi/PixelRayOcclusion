using Google.FlatBuffers;
using PRO.Flat.Ex;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;
using PRO.EventData;

namespace PRO
{
    public class RoleInfo
    {
        private static Dictionary<AgentNavMould.Mould, AgentNavMould> NavMouldDic = new Dictionary<AgentNavMould.Mould, AgentNavMould>()
        {
            { new AgentNavMould.Mould(Vector2Int.zero , Vector2Int.zero) , null }
        };
        public static AgentNavMould GetNavMould(AgentNavMould.Mould mould)
        {
            if (NavMouldDic.TryGetValue(mould, out var value)) return value;
            else
            {
                value = new AgentNavMould(mould);
                NavMouldDic.Add(mould, value);
                return value;
            }
        }

        #region RoleInfo对象池
        private static ObjectPool<RoleInfo> pool = new ObjectPool<RoleInfo>();
        public readonly static RoleInfo empty = new RoleInfo();
        public static RoleInfo TakeOut() => pool.TakeOut();
        public static void PutIn(RoleInfo info)
        {
            info.ClearEvent();
            CloneValue(empty, info);
            pool.PutIn(info);
        }
        #endregion

        public EventData_Int32 最大血量;//1
        public EventData_Int32 血量;//2
        public EventData_Int32 最大护甲;//3
        public EventData_Int32 护甲;//4
        public EventData_Int32 攻击力;//5
        public EventData_Double 移动速度;//6     //移动速度,走一个格子需要多少秒
        public EventData_Double 攻击速度;//7
        public EventData_Int32[] 抗性Array = new EventData_Int32[(int)战斗效果属性.end];//8
        public EventData_Double 命中率;//9
        public EventData_Double 闪避率;//10
        public EventData_Double 暴击率;//11
        public EventData_Int32 韧性;//12
        public AgentNavMould NavMould;//13

        public RoleInfo()
        {
            最大血量 = new(this);
            血量 = new(this);
            最大护甲 = new(this);
            护甲 = new(this);
            攻击力 = new(this);
            移动速度 = new(this);
            攻击速度 = new(this);
            抗性Array = new EventData_Int32[(int)战斗效果属性.end];
            for (int i = 0; i < 抗性Array.Length; i++)
                抗性Array[i] = new(this);
            命中率 = new(this);
            闪避率 = new(this);
            暴击率 = new(this);
            韧性 = new(this);

            血量.AddValueChangeEvent(EventClear.UnClear,
                (info, changeValue, oldData, newData) =>
                {
                    if (newData.Value > info.最大血量.ValueSum)
                    {
                        changeValue.EventData.GetValue_额外((long)DefaultValueID.溢出修正).Value = newData.Value - info.最大血量.ValueSum;
                    }
                });
            护甲.AddValueChangeEvent(EventClear.UnClear,
                (info, changeValue, oldData, newData) =>
                {
                    if (newData.Value > info.最大护甲.ValueSum)
                    {
                        changeValue.EventData.GetValue_额外((long)DefaultValueID.溢出修正).Value = newData.Value - info.最大护甲.ValueSum;
                    }
                });
        }

        public void ClearEvent()
        {
            最大血量.ClearEvent();
            血量.ClearEvent();
            最大护甲.ClearEvent();
            护甲.ClearEvent();
            攻击力.ClearEvent();
            移动速度.ClearEvent();
            攻击速度.ClearEvent();
            for (int i = 0; i < 抗性Array.Length; i++)
                抗性Array[i].ClearEvent();
            闪避率.ClearEvent();
            暴击率.ClearEvent();
            韧性.ClearEvent();
        }

        public static void CloneValue(RoleInfo from, RoleInfo to)
        {
            EventData_Int32.CloneValue(from.最大血量, to.最大血量);
            EventData_Int32.CloneValue(from.血量, to.血量);
            EventData_Int32.CloneValue(from.最大护甲, to.最大护甲);
            EventData_Int32.CloneValue(from.护甲, to.护甲);
            EventData_Int32.CloneValue(from.攻击力, to.攻击力);
            EventData_Double.CloneValue(from.移动速度, to.移动速度);
            EventData_Double.CloneValue(from.攻击速度, to.攻击速度);
            for (int i = 0; i < (int)战斗效果属性.end; i++)
                EventData_Int32.CloneValue(from.抗性Array[i], to.抗性Array[i]);
            EventData_Double.CloneValue(from.闪避率, to.闪避率);
            EventData_Double.CloneValue(from.暴击率, to.暴击率);
            EventData_Int32.CloneValue(from.韧性, to.韧性);
            to.NavMould = from.NavMould;
        }
        //public void Add(RoleInfo info)
        //{
        //    最大血量.Add(info.最大血量);
        //    血量.Add(info.血量);
        //    最大护甲.Add(info.最大护甲);
        //    护甲.Add(info.护甲);
        //    攻击力.Add(info.攻击力);
        //    移动速度.Add(info.移动速度);
        //    攻击速度.Add(info.攻击速度);
        //    for (int i = 0; i < 抗性Array.Length; i++)
        //        抗性Array[i].Add(info.抗性Array[i]);
        //    闪避率.Add(info.闪避率);
        //    暴击率.Add(info.暴击率);
        //    韧性.Add(info.韧性);
        //}
        //public void Subtract(RoleInfo info)
        //{
        //    最大血量.Subtract(info.最大血量);
        //    血量.Subtract(info.血量);
        //    最大护甲.Subtract(info.最大护甲);
        //    护甲.Subtract(info.护甲);
        //    攻击力.Subtract(info.攻击力);
        //    移动速度.Subtract(info.移动速度);
        //    攻击速度.Subtract(info.攻击速度);
        //    for (int i = 0; i < 抗性Array.Length; i++)
        //        抗性Array[i].Subtract(info.抗性Array[i]);
        //    闪避率.Subtract(info.闪避率);
        //    暴击率.Subtract(info.暴击率);
        //    韧性.Subtract(info.韧性);
        //}
        //public void Multiply(double value)
        //{
        //    最大血量.Multiply(value);
        //    血量.Multiply(value);
        //    最大护甲.Multiply(value);
        //    护甲.Multiply(value);
        //    攻击力.Multiply(value);
        //    移动速度.Multiply(value);
        //    攻击速度.Multiply(value);
        //    for (int i = 0; i < 抗性Array.Length; i++)
        //        抗性Array[i].Multiply(value);
        //    闪避率.Multiply(value);
        //    暴击率.Multiply(value);
        //    韧性.Multiply(value);
        //}

        public Offset<Flat.RoleInfoData> ToDisk(FlatBufferBuilder builder)
        {
            Flat.RoleInfoData.StartValue8Vector(builder, 抗性Array.Length);
            for (int i = 0; i < 抗性Array.Length; i++)
                抗性Array[i].ToDisk(builder);
            var 抗性ArrayOffset = builder.EndVector();

            Flat.RoleInfoData.StartRoleInfoData(builder);
            Flat.RoleInfoData.AddValue1(builder, 最大血量.ToDisk(builder));
            Flat.RoleInfoData.AddValue2(builder, 血量.ToDisk(builder));
            Flat.RoleInfoData.AddValue3(builder, 最大护甲.ToDisk(builder));
            Flat.RoleInfoData.AddValue4(builder, 护甲.ToDisk(builder));
            Flat.RoleInfoData.AddValue5(builder, 攻击力.ToDisk(builder));
            Flat.RoleInfoData.AddValue6(builder, 移动速度.ToDisk(builder));
            Flat.RoleInfoData.AddValue7(builder, 攻击速度.ToDisk(builder));
            Flat.RoleInfoData.AddValue8(builder, 抗性ArrayOffset);
            Flat.RoleInfoData.AddValue9(builder, 命中率.ToDisk(builder));
            Flat.RoleInfoData.AddValue10(builder, 闪避率.ToDisk(builder));
            Flat.RoleInfoData.AddValue11(builder, 暴击率.ToDisk(builder));
            Flat.RoleInfoData.AddValue12(builder, 韧性.ToDisk(builder));
            if (NavMould != null)
            {
                Flat.RoleInfoData.AddMouldSize(builder, NavMould.mould.size.ToDisk(builder));
                Flat.RoleInfoData.AddMouldOffset(builder, NavMould.mould.offset.ToDisk(builder));
            }
            else
            {
                Flat.RoleInfoData.AddMouldSize(builder, Vector2Int.zero.ToDisk(builder));
                Flat.RoleInfoData.AddMouldOffset(builder, Vector2Int.zero.ToDisk(builder));
            }
            return Flat.RoleInfoData.EndRoleInfoData(builder);
        }

        public void ToRAM(Flat.RoleInfoData diskData)
        {
            最大血量.ToRAM(diskData.Value1.Value);
            血量.ToRAM(diskData.Value2.Value);
            最大护甲.ToRAM(diskData.Value3.Value);
            护甲.ToRAM(diskData.Value4.Value);
            攻击力.ToRAM(diskData.Value5.Value);
            移动速度.ToRAM(diskData.Value6.Value);
            攻击速度.ToRAM(diskData.Value7.Value);
            for (int i = (int)战斗效果属性.end - 1; i >= 0; i--)
                抗性Array[i].ToRAM(diskData.Value8(i).Value);
            命中率.ToRAM(diskData.Value9.Value);
            闪避率.ToRAM(diskData.Value10.Value);
            暴击率.ToRAM(diskData.Value11.Value);
            韧性.ToRAM(diskData.Value12.Value);

            NavMould = GetNavMould(new AgentNavMould.Mould(diskData.MouldSize.Value.ToRAM(), diskData.MouldOffset.Value.ToRAM()));
        }
    }

    public enum Toward
    {
        right,
        left
    }
}
