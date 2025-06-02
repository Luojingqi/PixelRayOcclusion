using PRO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class SkillPointer_范围内选择类 : SkillPointerBase
    {
        public class OnePointer
        {
            public Sprite sprite;
        }
        [OdinSerialize]
        [ShowInInspector]
        private List<OnePointer> pointerList = new List<OnePointer>();

        public OnePointer NowPointer { get; private set; }

        public void SetPointer(int Radius_G)
        {
            NowPointer = pointerList[Radius_G];
            RangeBox.sprite = NowPointer.sprite;
        }

        [Button("填充")]
        public void Editor(params Sprite[] sprites)
        {
            pointerList.Clear();
            for (int i = 0; i < sprites.Length; i++)
            {
                pointerList.Add(new OnePointer()
                {
                    sprite = sprites[i],
                }); 
            }
        }
    }
}
