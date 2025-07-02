using PRO.Skill;
using PRO.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class RoleManager : MonoScriptBase, ITime_Awake
    {
        public static RoleManager Inst { get; private set; }
        private Dictionary<string, GameObjectPool<Role>> rolePoolDic = new Dictionary<string, GameObjectPool<Role>>();

        private Transform roleNode;
        public void TimeAwake()
        {
            Inst = this;
            roleNode = new GameObject("RolePool").transform;
            roleNode.parent = SceneManager.Inst.PoolNode;
            AddRolePool(AssetManager.Load_A<GameObject>("role.ab", @$"ScriptsAssembly\PRO\Role\Role_默认").GetComponent<Role>());
        }

        private void AddRolePool(Role role)
        {
            var node = new GameObject(role.RoleTypeName + "PoolNode").transform;
            node.transform.parent = roleNode;
            var pool = new GameObjectPool<Role>(role, node);
            pool.CreateEvent += t => t.Init();
            rolePoolDic.Add(role.RoleTypeName, pool);
        }

        public Role TakeOut(string roleTypeName, SceneEntity scene)
        {
            var role = rolePoolDic[roleTypeName].TakeOut();
            role.TakeOut(scene);
            scene.Role_Guid_Dic.Add(role.Guid, role);
            scene.Role_Trans_Dic.Add(role.transform, role);
            return role;
        }

        public void PutIn(Role role)
        {
            role.Scene.Role_Guid_Dic.Remove(role.Guid);
            role.Scene.Role_Trans_Dic.Remove(role.transform);
            rolePoolDic[role.RoleTypeName].PutIn(role);
            role.PutIn();
        }


        public readonly SkillPointerAsset SkillPointerAsset;
    }
}
