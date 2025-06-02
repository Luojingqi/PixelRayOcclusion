using PRO.Buff.Base.IBuff;
using PRO;
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
            pool = new GameObjectPool<BuffInfoC>(view.buffInfo.gameObject, view.buffInfoPanel.transform);
            pool.CreateEventT += (g, t) => { t.Init(); };

        }

        private List<BuffInfoC> InfoList = new List<BuffInfoC>();
        public void SetRole(Role role)
        {
            Clear();
            foreach (var buffList in role.AllBuff)
                foreach (var buff in buffList)
                    if (buff.GetActive() && buff is IBuff_UI)
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
            BuffInfoC info = pool.TakeOutT();
            info.transform.parent = transform;
            return info;
        }
        public void PutIn(BuffInfoC info)
        {
            info.Clear();
            pool.PutIn(info.gameObject);
        }
    }
}
