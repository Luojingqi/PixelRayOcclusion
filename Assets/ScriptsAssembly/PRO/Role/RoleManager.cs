using PRO.Skill;
using PRO.Tool;
using PRO.Tool.Serialize.IO;
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
            {
                var role = AssetManager.Load_A<GameObject>("role.ab", @$"ScriptsAssembly\PRO\Role\Role_默认").GetComponent<Role>();
                role.nav = NavManager.Inst.GetNav("默认");
                var pool = AddRolePool(role);
                pool.PutInEvent += t =>
                {
                    if (t.nav != null && t.nav.TypeName != "默认")
                        t.nav = NavManager.Inst.GetNav("默认");
                };
            }
        }

        private GameObjectPool<Role> AddRolePool(Role role)
        {
            var node = new GameObject(role.RoleTypeName + "PoolNode").transform;
            node.transform.parent = roleNode;
            var pool = new GameObjectPool<Role>(role, node);
            pool.CreateEvent += t => t.Init();
            rolePoolDic.Add(role.RoleTypeName, pool);
            return pool;
        }

        public Role TakeOut(string roleTypeName, SceneEntity scene, string guid)
        {
            var role = rolePoolDic[roleTypeName].TakeOut();
            role.TakeOut(scene, guid);
            scene.ActiveRole_Guid.Add(role.GUID, role);
            scene.ActiveRole_Trans.Add(role.transform, role);
            return role;
        }

        public void PutIn(Role role)
        {
            role.Scene.ActiveRole_Guid.Remove(role.GUID);
            role.Scene.ActiveRole_Trans.Remove(role.transform);
            rolePoolDic[role.RoleTypeName].PutIn(role);
            role.PutIn();
        }

        public Role Load(string guid, SceneEntity scene)
        {
            Role role = null;
            if (scene.ActiveRole_Guid.ContainsKey(guid)) return role;
            if (IOTool.LoadFlat(@$"{scene.sceneCatalog.directoryInfo}\Role\{guid}", out var builder))
            {
                var diskData = Flat.RoleData.GetRootAsRoleData(builder.DataBuffer);
                role = TakeOut(diskData.RoleTypeName, scene, guid);
                role.ToRAM(diskData);
            }
            return role;
        }


        public readonly SkillPointerAsset SkillPointerAsset;
    }
}
