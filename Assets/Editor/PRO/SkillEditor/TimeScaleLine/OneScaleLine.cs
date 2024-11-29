using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class OneScaleLine
    {
        public VisualElement View { get; private set; }
        private Label TimeText;
        private VisualElement ScaleLine;
        public OneScaleLine()
        {
            View = new VisualElement();
            uxml.CloneTree(View);
            View = View.Q<VisualElement>("OneScaleLine");
            TimeText = View.Q<Label>("TimeText");
            ScaleLine = View.Q<VisualElement>("ScaleLine");
        }

        public void SetText(string text) => TimeText.text = text;
        /// <summary>
        /// 设置刻度线的长度
        /// </summary>
        /// <param name="scale"></param>
        public void SetScaleLine(ScaleLineType scale)
        {
            switch (scale)
            {
                case ScaleLineType.s: ScaleLine.style.height = new StyleLength(new Length(30, LengthUnit.Percent)); break;
                case ScaleLineType.l: ScaleLine.style.height = new StyleLength(new Length(45, LengthUnit.Percent)); break;
            }
        }
        /// <summary>
        /// 设置刻度线间隔
        /// </summary>
        public void SetWidth(int pixel)
        {
            View.style.width = new StyleLength(new Length(pixel, LengthUnit.Pixel));
            ScaleLine.style.width = new StyleLength(new Length(pixel / 2, LengthUnit.Pixel));
            TimeText.style.width = new StyleLength(new Length(pixel, LengthUnit.Pixel));
        }

        public enum ScaleLineType
        {
            s,
            l,
        }



        private static VisualTreeAsset uxml
        {
            get
            {
                if (_uxml != null) return _uxml;
                else { _uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath); return _uxml; }
            }
        }
        private static VisualTreeAsset _uxml;
        private static string uxmlPath = "Assets/Editor/PRO/SkillEditor/TimeScaleLine/OneScaleLine.uxml";
    }
}