using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace PRO
{
    /// <summary>
    ///  背包存档序列化与反序列化
    /// </summary>
    public class Bag_Disk
    {
        public string name;
        public string time;
        public Dictionary<string, List<Prop_Disk>> infos;
    }

    /// <summary>
    /// 道具存储的基类
    /// </summary>
    public abstract class Prop_Disk
    {
        public abstract string type { get; }
    }

    /// <summary>
    /// 具有数量属性的道具
    /// </summary>
    public class Prop_Disk_Number : Prop_Disk
    {
        public override string type => "数量";


        private int propNumber;
        /// <summary>
        /// 实际拥有数量
        /// </summary>
        public int PropNumber { get { return propNumber; } set { propNumber = value; PropNumberChangesEvent?.Invoke(propNumber); } }
        private event Action<int> PropNumberChangesEvent;
        public void AddPropNumberChangesEvent(Action<int> PropNumberChangesEvent) => this.PropNumberChangesEvent += PropNumberChangesEvent;
        public void RemovePropNumberChangesEvent(Action<int> PropNumberChangesEvent) => this.PropNumberChangesEvent -= PropNumberChangesEvent;

        private int propNumberExpect;
        /// <summary>
        /// 预期拥有数量
        /// </summary>
        public int PropNumberExpect { get { return propNumberExpect; } set { propNumberExpect = value; PropNumberExpectChangesEvent?.Invoke(propNumberExpect); } }
        private event Action<int> PropNumberExpectChangesEvent;
        public void AddPropNumberExpectChangesEvent(Action<int> PropNumberExpectChangesEvent) => this.PropNumberExpectChangesEvent += PropNumberExpectChangesEvent;
        public void RemovePropNumberExpectChangesEvent(Action<int> PropNumberExpectChangesEvent) => this.PropNumberExpectChangesEvent -= PropNumberExpectChangesEvent;

        public void ResetEvent()
        {
            PropNumberChangesEvent = null;
            PropNumberExpectChangesEvent = null;
        }
    }


    public class Prop_Disk_Read : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class Prop_Disk_Write : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}