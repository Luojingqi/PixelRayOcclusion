namespace PRO.TurnBased
{
    /// <summary>
    /// 单个回合的状态
    /// </summary>
    public enum TurnStateEnum
    {
        /// <summary>
        /// 准备阶段，处理角色身上的buff
        /// </summary>
        start,
        /// <summary>
        /// 操作阶段，等待并执行操作
        /// </summary>
        operate,
        /// <summary>
        /// 结束阶段
        /// </summary>
        end,

    }
}
