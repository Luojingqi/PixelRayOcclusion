using PRO.DataStructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PRO.Tool
{
    //[CreateAssetMenu(menuName = "创建")]
    public class MonoScriptPriority : SerializedScriptableObject
    {
        public class Item
        {
#if UNITY_EDITOR
            [GUIColor("GetColor")]
            [ReadOnly]
            public bool Awake;
            [GUIColor("GetColor")]
            [ReadOnly]
            public bool Start;
            [GUIColor("GetColor")]
            [ReadOnly]
            public bool Update;
            [GUIColor("GetColor")]
            [ReadOnly]
            public bool LateUpdate;


            [ReadOnly]
            public MonoScript script;
#endif
            public int priority = -1;

            private static Color GetColor(bool b)
            {
                return b ? Color.green : Color.red;
            }
        }
        /// <summary>
        /// 用于运行时通过type获取item
        /// </summary>
        [HideInInspector]
        public Dictionary<Type, Item> typeDic = new Dictionary<Type, Item>();
#if UNITY_EDITOR
        /// <summary>
        /// 用于编辑器时记录item防止丢失
        /// </summary>
        [HideInInspector]
        public Dictionary<MonoScript, Item> assetDic = new Dictionary<MonoScript, Item>();

        [TableList]
        public List<Item> list = new List<Item>();
        private PriorityQueue<Item> queue = new PriorityQueue<Item>();
        [Button("更新")]
        public void Update()
        {
            list.Clear();
            typeDic.Clear();
            // 1. 查找所有类型为 MonoScript 的资源 GUID
            string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/ScriptsAssembly", "Assets/Editor/PRO" });
            foreach (string guid in guids)
            {
                // 2. 通过 GUID 获取资源路径
                string path = AssetDatabase.GUIDToAssetPath(guid);
                //// 3. 加载资源并检查是否为 MonoBehaviour
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null)
                {
                    // 4. 检查脚本是否继承自 MonoBehaviour
                    System.Type type = script.GetClass();
                    if (type != null && typeof(MonoScriptBase).IsAssignableFrom(type) && type.IsAbstract == false)
                    {
                        if (assetDic.TryGetValue(script, out var item) == false)
                        {
                            item = new Item();
                            item.script = script;
                        }

                        item.Awake = typeof(ITime_Awake).IsAssignableFrom(type);
                        item.Start = typeof(ITime_Start).IsAssignableFrom(type);
                        item.Update = typeof(ITime_Update).IsAssignableFrom(type);
                        item.LateUpdate = typeof(ITime_LateUpdate).IsAssignableFrom(type);

                        queue.Enqueue(item, item.priority);
                    }
                }
            }
            assetDic.Clear();
            typeDic.Clear();
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                assetDic.Add(item.script, item);
                typeDic.Add(item.script.GetClass(), item);
                list.Add(item);
            }
        }
#endif
    }
}
