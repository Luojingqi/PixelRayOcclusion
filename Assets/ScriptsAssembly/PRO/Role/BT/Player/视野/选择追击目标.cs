using NodeCanvas.Framework;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.BT.视野
{
    public class 选择追击目标 : ConditionTask
    {
        public enum 选择追击目标策略枚举
        {
            随机,
            绝对距离最近,
            绝对血量与护甲最少,
        }

        public BBParameter<Role> Agent;
        public BBParameter<Dictionary<string, Role>> 看到的角色Dic;
        public BBParameter<Role> 追击目标;
        public BBParameter<选择追击目标策略枚举> 选择追击目标策略;
        public BBParameter<Vector2Int> 移动目标;

        protected override void OnEnable()
        {
            if (追击目标.value != null && 看到的角色Dic.value.ContainsKey(追击目标.value.GUID))
                return;
            else
            {
                追击目标.value = null;
                switch (选择追击目标策略.value)
                {
                    case 选择追击目标策略枚举.随机:
                        {
                            int random = Random.Range(0, 看到的角色Dic.value.Count);
                            int index = 0;
                            foreach (var role in 看到的角色Dic.value.Values)
                            {
                                if (index == random)
                                    追击目标.value = role;
                                else index++;
                            }
                            break;
                        }
                    case 选择追击目标策略枚举.绝对距离最近:
                        {
                            var 距离最近的距离 = float.MaxValue;
                            Role 距离最近的角色 = null;
                            foreach (var role in 看到的角色Dic.value.Values)
                            {
                                var 距离 = Vector2.Distance(Agent.value.transform.position, role.transform.position);
                                if (距离 < 距离最近的距离)
                                    距离最近的角色 = role;
                            }
                            追击目标.value = 距离最近的角色;
                            break;
                        }
                    case 选择追击目标策略枚举.绝对血量与护甲最少:
                        {
                            var 绝对血量与护甲最少的值 = int.MaxValue;
                            Role 绝对血量与护甲最少的角色 = null;
                            foreach (var role in 看到的角色Dic.value.Values)
                            {
                                var 绝对血量与护甲 = role.Info.血量.Value + role.Info.护甲.Value;
                                if (绝对血量与护甲 < 绝对血量与护甲最少的值)
                                    绝对血量与护甲最少的角色 = role;
                            }
                            追击目标.value = 绝对血量与护甲最少的角色;
                            break;
                        }
                }
            }
        }

        protected override bool OnCheck()
        {
            if (追击目标.value != null)
                移动目标.value = 追击目标.value.GlobalPos;
            return 追击目标.value != null;
        }
    }
}
