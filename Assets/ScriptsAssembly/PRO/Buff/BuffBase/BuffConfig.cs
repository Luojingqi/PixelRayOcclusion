using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.Buff.Base
{
    [CreateAssetMenu(menuName = "创建一个Buff配置")]
    public class BuffConfig : SerializedScriptableObject
    {
        public string Name;
        public int Priority;
        public bool 独有;
        public bool ui;
        public string 说明文本;
    }
}
