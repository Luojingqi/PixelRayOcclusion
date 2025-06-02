using PRO;
using System.Collections.Generic;

namespace PRO
{
    public static class NavManager
    {
        private static Dictionary<string, Nav> NavDic = new Dictionary<string, Nav>();

        public static void AddNav(Nav nav) => NavDic.Add(nav.TypeName, nav);

        public static Nav GetNav(string typeName) => NavDic[typeName];

        public static void Init()
        {
            AddNav(new Nav("默认", new NavAgentMould(new(2, 7), new(1, 0))));
        }
    }
}
