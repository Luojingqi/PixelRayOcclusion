using System.Collections.Generic;

namespace PRO
{
    public class NavManager : MonoScriptBase, ITime_Awake
    {
        public static NavManager Inst { get; private set; }
        private Dictionary<string, Nav> NavDic = new Dictionary<string, Nav>();

        public void AddNav(Nav nav) => NavDic.Add(nav.TypeName, nav);

        public Nav GetNav(string typeName) => NavDic[typeName];


        public void TimeAwake()
        {
            Inst = this;
            AddNav(new Nav("默认", new NavAgentMould(new(2, 7), new(1, 0))));
        }
    }
}
