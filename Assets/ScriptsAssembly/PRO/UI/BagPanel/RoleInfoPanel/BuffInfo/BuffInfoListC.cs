using PRO.Tool;
using System.Collections.Generic;

namespace PRO
{
    public class BuffInfoListC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private BuffInfoListV view = new BuffInfoListV();
        private GameObjectPool<BuffInfoC> pool;
        public override void Init()
        {
            base.Init();
            AddChildUI(view.buffInfoPanel);
            pool = new GameObjectPool<BuffInfoC>(view.buffInfo, view.buffInfoPanel.transform);
            pool.CreateEvent += t => { t.Init(); };

        }

        private List<BuffInfoC> InfoList = new List<BuffInfoC>();
        public void SetRole(Role role)
        {
            Clear();
            foreach (var buffSortList in role.AllBuff)
                foreach (var buffList in buffSortList)
                    foreach (var buff in buffList)
                        if (buff.Config.ui)
                        {
                            BuffInfoC info = TakeOut();
                            info.SetBuff(buff);
                            InfoList.Add(info);
                        }
        }
        public void Clear()
        {
            foreach (var info in InfoList)
                PutIn(info);
            InfoList.Clear();
        }
        public BuffInfoC TakeOut()
        {
            BuffInfoC info = pool.TakeOut();
            info.transform.SetParent(transform);
            return info;
        }
        public void PutIn(BuffInfoC info)
        {
            info.Clear();
            pool.PutIn(info);
        }
    }
}
