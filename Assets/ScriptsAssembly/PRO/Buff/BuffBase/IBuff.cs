namespace PRO.Buff.Base
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
    public interface IBuff_倒计时
    {
        public float Countdown { get; set; }
        public float CountdownMax { get; set; }
    }
}