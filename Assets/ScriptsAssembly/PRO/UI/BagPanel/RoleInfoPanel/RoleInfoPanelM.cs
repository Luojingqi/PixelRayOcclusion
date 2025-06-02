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


        public class �ж���Panel
        {
            GameObjectPool<Transform> �ж���_����_Pool;
            GameObjectPool<Transform> �ж���_������_Pool;
            Transform �ж���_����ֵ;
            Transform �ж���_������ֵ;
            TMP_Text �ж���_����ֵ_Text;
            TMP_Text �ж���_������ֵ_Text;
            public �ж���Panel(Transform transform)
            {
                Transform �ж���List = transform.Find("�ж���List");
                �ж���_����_Pool = new GameObjectPool<Transform>(�ж���List.Find("�ж���-����").gameObject, �ж���List);
                �ж���_������_Pool = new GameObjectPool<Transform>(�ж���List.Find("�ж���-������").gameObject, �ж���List);
                �ж���_����ֵ = �ж���List.Find("�ж���-����ֵ");
                �ж���_������ֵ = �ж���List.Find("�ж���-������ֵ");
                �ж���_����ֵ_Text = �ж���_����ֵ.Find("Value").GetComponent<TMP_Text>();
                �ж���_������ֵ_Text = �ж���_����ֵ.Find("Value").GetComponent<TMP_Text>();
            }
            List<Transform> �ж���_����_ActiveList = new List<Transform>();
            List<Transform> �ж���_������_ActiveList = new List<Transform>();

            public int now�����ж��� => �ж���_����_ActiveList.Count;
            public int now�������ж��� => �ж���_������_ActiveList.Count;
            public int nowȫ���ж��� => �ж���_����_ActiveList.Count + �ж���_������_ActiveList.Count;
            public void SetValue(int �����ж���, int ȫ���ж���)
            {
                if (�����ж��� < 0 || ȫ���ж��� < 0 || �����ж��� > ȫ���ж���) return;
                foreach (var t in �ж���_����_ActiveList) �ж���_����_Pool.PutIn(t.gameObject);
                foreach (var t in �ж���_������_ActiveList) �ж���_������_Pool.PutIn(t.gameObject);
                �ж���_����_ActiveList.Clear();
                �ж���_������_ActiveList.Clear();

                if (ȫ���ж��� <= 7)
                {
                    for (int i = 0; i < ȫ���ж���; i++)
                    {
                        if (i < �����ж���) �ж���_����_ActiveList.Add(�ж���_����_Pool.TakeOutT());
                        else �ж���_������_ActiveList.Add(�ж���_������_Pool.TakeOutT());
                    }
                    �ж���_����ֵ.gameObject.SetActive(false);
                    �ж���_������ֵ.gameObject.SetActive(false);
                }
                else
                {
                    �ж���_����ֵ.gameObject.SetActive(true);
                    �ж���_������ֵ.gameObject.SetActive(true);
                    �ж���_����ֵ_Text.text = $"{�����ж���}x";
                    �ж���_������ֵ_Text.text = $"{ȫ���ж��� - �����ж���}x";
                }
            }
        }
    }
}
