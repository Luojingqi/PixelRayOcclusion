namespace PRO
{
    /// <summary>
    /// 战斗起始效果数据
    /// </summary>
    public struct StartCombatEffectData
    {
        public int value;
        public 属性 type;

        public StartCombatEffectData(属性 type, int value)
        {
            this.value = value;
            this.type = type;
        }
    }
    /// <summary>
    /// 战斗最终效果数据
    /// </summary>
    public struct EndCombatEffectData
    {
        public bool is命中;
        public bool is暴击;

        public float 命中率;
        public int 护甲;
        public int 血量;

        public float 暴击率;
        public int 护甲_暴击;
        public int 血量_暴击;
    }
    /// <summary>
    /// 战斗效果的属性
    /// </summary>
    public enum 属性
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

        end
    }

    public enum 施法方式
    {
        直接触发,
        近战,
        远程
    }
}
