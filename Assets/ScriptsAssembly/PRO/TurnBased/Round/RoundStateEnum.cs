namespace PRO.TurnBased
{
    /// <summary>
    /// 单个轮次的状态
    /// </summary>
    public enum RoundStateEnum
    {
        /// <summary>
        /// 数据准备初始化阶段，游戏内不体现，仅代码层位当前轮次提供数据
        /// </summary>
        dataReady,
        /// <summary>
        /// 受惊判断，判断哪些角色会处于受惊状态
        /// </summary>
        beFrightened,
        /// <summary>
        /// 先攻判断，根据角色先攻值对回合进行排序
        /// </summary>
        initiative,
        /// <summary>
        /// 回合开始，依次执行每名角色的回合
        /// </summary>
        turn,
        /// <summary>
        /// 战斗结束
        /// </summary>
        end,
    }
}
