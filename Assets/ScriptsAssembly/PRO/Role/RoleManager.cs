using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public static class RoleManager
    {

        private static Dictionary<string, MonoObjectPool<Role>> rolePoolDic = new Dictionary<string, MonoObjectPool<Role>>();

        private static Transform nodeTransform;
        public static void Init()
        {
            nodeTransform = new GameObject("RolePoolNode").transform;
            //  nodeTransform.parent=  GameObject.Find()
        }

        private static void AddRolePool(Role role)
        {
            var node = new GameObject(role.RoleTypeName + "PoolNode").transform;
            node.transform.parent = nodeTransform;
            var pool = new MonoObjectPool<Role>(role, node);
            rolePoolDic.Add(role.RoleTypeName, pool);
        }

        public static Role TakeOut(string roleTypeName)
        {
            var role = rolePoolDic[roleTypeName].TakeOut();

            return role;
        }

        public static void PutIn(Role role)
        {
            rolePoolDic[role.RoleTypeName].PutIn(role);
        }
    }
}
