using Google.FlatBuffers;

namespace PRO
{
    /// <summary>
    /// 战斗起始效果数据
    /// </summary>
    public struct StartCombatEffectData
    {
        public 战斗效果属性 type;
        public int value;

        public StartCombatEffectData(战斗效果属性 type, int value)
        {
            this.value = value;
            this.type = type;
        }

        public Offset<Flat.StartCombatEffectData> ToDisk(FlatBufferBuilder builder)
        {
            return Flat.StartCombatEffectData.CreateStartCombatEffectData(builder, value, (int)type);
        }
        public static StartCombatEffectData ToRAM(Flat.StartCombatEffectData diskData)
        {
            return new StartCombatEffectData((战斗效果属性)diskData.Type, diskData.Value);
        }
    }
    /// <summary>
    /// 战斗最终效果数据
    /// </summary>
    public struct EndCombatEffectData
    {
        public bool is命中;
        public bool is暴击;

        public double 命中率;
        public int 护甲;
        public int 血量;

        public double 暴击率;
        public int 护甲_暴击;
        public int 血量_暴击;
    }
    /// <summary>
    /// 战斗效果的属性
    /// </summary>
    public enum 战斗效果属性
    {
        无属性,
        切割,
        穿刺,
        冲击,
        真实,
        火,
        水,
        毒,
        电,
        冰,
        治疗,
        护甲修复,
        end
    }

    public enum 施法触发方式
    {
        直接,
        近战,
        远程
    }
}
