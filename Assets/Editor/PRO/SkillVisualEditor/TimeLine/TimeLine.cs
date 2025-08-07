using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    /// <summary>
    /// ʱ�������
    /// </summary>
    public class TimeLine
    {
        public VisualElement View { get; private set; }
        public TimeLine(VisualElement view)
        {
            View = view;
            Line = View.Q<VisualElement>("Line");

            Line.style.paddingLeft = new StyleLength(new Length(0, LengthUnit.Pixel));
        }

        public VisualElement Line { get; private set; }
        /// <summary>
        /// ���ص�ƫ��
        /// </summary>
        public float Offset
        {
            get
            {
                return Line.style.marginLeft.value.value;
            }
            set
            {
                Line.style.marginLeft = new StyleLength(new Length(value, LengthUnit.Pixel));
            }
        }


    }
}
