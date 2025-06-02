using PRO.Tool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PRO
{
    public class RoleInfoPanelM
    {
        public class Info
        {
            public TMP_Text Name;
            public TMP_Text Value;
            public Info(Transform transform)
            {
                Name = transform.Find("Name").GetComponent<TMP_Text>();
                Value = transform.Find("Value").GetComponent<TMP_Text>();
            }
        }


        public class 行动点Panel
        {
            GameObjectPool<Transform> 行动点_可用_Pool;
            GameObjectPool<Transform> 行动点_不可用_Pool;
            Transform 行动点_可用值;
            Transform 行动点_不可用值;
            TMP_Text 行动点_可用值_Text;
            TMP_Text 行动点_不可用值_Text;
            public 行动点Panel(Transform transform)
            {
                Transform 行动点List = transform.Find("行动点List");
                行动点_可用_Pool = new GameObjectPool<Transform>(行动点List.Find("行动点-可用").gameObject, 行动点List);
                行动点_不可用_Pool = new GameObjectPool<Transform>(行动点List.Find("行动点-不可用").gameObject, 行动点List);
                行动点_可用值 = 行动点List.Find("行动点-可用值");
                行动点_不可用值 = 行动点List.Find("行动点-不可用值");
                行动点_可用值_Text = 行动点_可用值.Find("Value").GetComponent<TMP_Text>();
                行动点_不可用值_Text = 行动点_可用值.Find("Value").GetComponent<TMP_Text>();
            }
            List<Transform> 行动点_可用_ActiveList = new List<Transform>();
            List<Transform> 行动点_不可用_ActiveList = new List<Transform>();

            public int now可用行动点 => 行动点_可用_ActiveList.Count;
            public int now不可用行动点 => 行动点_不可用_ActiveList.Count;
            public int now全部行动点 => 行动点_可用_ActiveList.Count + 行动点_不可用_ActiveList.Count;
            public void SetValue(int 可用行动点, int 全部行动点)
            {
                if (可用行动点 < 0 || 全部行动点 < 0 || 可用行动点 > 全部行动点) return;
                foreach (var t in 行动点_可用_ActiveList) 行动点_可用_Pool.PutIn(t.gameObject);
                foreach (var t in 行动点_不可用_ActiveList) 行动点_不可用_Pool.PutIn(t.gameObject);
                行动点_可用_ActiveList.Clear();
                行动点_不可用_ActiveList.Clear();

                if (全部行动点 <= 7)
                {
                    for (int i = 0; i < 全部行动点; i++)
                    {
                        if (i < 可用行动点) 行动点_可用_ActiveList.Add(行动点_可用_Pool.TakeOutT());
                        else 行动点_不可用_ActiveList.Add(行动点_不可用_Pool.TakeOutT());
                    }
                    行动点_可用值.gameObject.SetActive(false);
                    行动点_不可用值.gameObject.SetActive(false);
                }
                else
                {
                    行动点_可用值.gameObject.SetActive(true);
                    行动点_不可用值.gameObject.SetActive(true);
                    行动点_可用值_Text.text = $"{可用行动点}x";
                    行动点_不可用值_Text.text = $"{全部行动点 - 可用行动点}x";
                }
            }
        }
    }
}
