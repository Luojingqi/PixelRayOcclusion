namespace PRO.Buff.Base.IBuff
{
    public interface IBuff_叠加
    {
        /// <summary>
        /// 叠加次数
        /// </summary>
        public int StackNumber { get; set; }
        /// <summary>
        /// 最大可叠加次数
        /// </summary>
        public int StackNumberMax { get; set; }
    }
    public interface IBuff_回合
    {
        /// <summary>
        /// 持续回合
        /// </summary>
        public int Round { get; set; }
    }
    public interface IBuff_比例
    {
        /// <summary>
        /// 比例
        /// </summary>
        public float Proportion { get; set; }
        /// <summary>
        /// 最大比例上限
        /// </summary>
        public float ProportionMax { get; set; }
    }
    public interface IBuff_UI
    {
        public string Info { get; }
    }
    /// <summary>
    /// 代表此buff同时只能存在一个
    /// </summary>
    public interface IBuff_独有
    {
        
    }
}