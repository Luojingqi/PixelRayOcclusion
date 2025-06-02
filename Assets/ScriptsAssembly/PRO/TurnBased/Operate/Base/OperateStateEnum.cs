namespace PRO.TurnBased
{
    public enum OperateStateEnum
    {
        /// <summary>
        /// 检查此操作是否可以触发，创建技能指示器等
        /// </summary>
        t0,
        /// <summary>
        /// 等待技能的触发，更新技能指示器跟随指针等
        /// </summary>
        t1,
        /// <summary>
        /// 技能的实际触发过程
        /// </summary>
        t2,
    }
}
