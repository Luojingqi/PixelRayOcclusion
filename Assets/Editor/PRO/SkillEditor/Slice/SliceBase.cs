using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal abstract class SliceBase : SerializedScriptableObject //ScriptableObject
    {
        private static int margin = 4;
        public VisualElement View { get; private set; }
        private VisualElement SpriteView;
        private Label LabelView;
        public TrackBase Track;
        public SliceBase(SliceBase_Disk sliceDisk)
        {
            DiskData = sliceDisk;
            View = new VisualElement();
            View.name = "Slice";
            SpriteView = new VisualElement();
            View.Add(SpriteView);
            SpriteView.name = "Sprite";
            #region 初始化边框
            View.style.backgroundColor = new StyleColor(Color.gray);
            View.style.borderTopWidth = new StyleFloat(3);
            View.style.borderBottomWidth = new StyleFloat(3);
            View.style.borderLeftWidth = new StyleFloat(3);
            View.style.borderRightWidth = new StyleFloat(3);
            View.style.borderBottomLeftRadius = new StyleLength(new Length(8, LengthUnit.Pixel));
            View.style.borderBottomRightRadius = new StyleLength(new Length(8, LengthUnit.Pixel));
            #endregion
            #region 设置背景字
            LabelView = new Label();
            SpriteView.Add(LabelView);
            LabelView.style.color = new StyleColor(Color.black);
            LabelView.style.fontSize = new StyleLength(new Length(15, LengthUnit.Pixel));
            #endregion
            // Background = new StyleBackground();
            Enable = true;
            #region 鼠标事件
            View.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 0)
                    SkillEditorWindow.Inst.SelectSlice = this;
            });
            View.RegisterCallback<MouseLeaveEvent>(evt =>
            {

            });
            View.RegisterCallback<MouseDownEvent>(evt =>
            {

            });
            View.RegisterCallback<MouseUpEvent>(evt =>
            {

            });

            View.RegisterCallback<MouseMoveEvent>(evt =>
            {

            });
            View.RegisterCallback<WheelEvent>(evt =>
            {
                if (evt.mouseDelta.y > 0)
                {
                    Track.SetSliceFrameLength(StartFrame, FrameLength - 1);
                }
                else if (evt.mouseDelta.y < 0)
                {
                    Track.SetSliceFrameLength(StartFrame, FrameLength + 1);
                }
            });
            #endregion

            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("启用 | 禁用", _ => Enable = !Enable);

                evt.menu.AppendAction("删除切片", _ => Track.RemoveSlice(StartFrame));
            }));
        }


        [PropertyOrder(20000)]
        [BoxGroup]
        [LabelText("磁盘数据")]
        [ShowInInspector]
        public SliceBase_Disk DiskData { get; set; }

        [BoxGroup("切片基本信息")]
        [LabelText("名称")]
        [ShowInInspector]
        public string Name
        {
            get { return DiskData.name; }
            set { LabelView.text = value; DiskData.name = value; }
        }
        [BoxGroup("切片基本信息")]
        [LabelText("起始帧")]
        [ShowInInspector]
        public int StartFrame
        {
            get { return DiskData.startFrame; }
            set { DiskData.startFrame = value; Track.SwapSlice(StartFrame, value); }
        }
        [BoxGroup("切片基本信息")]
        [LabelText("帧长度")]
        [ShowInInspector]
        public int FrameLength
        {
            get { return DiskData.frameLength; }
            set { DiskData.frameLength = value; Track.SetSliceFrameLength(StartFrame, value); SetWidth(SkillEditorWindow.Inst.TimeScaleAxis.Spacing); }
        }
        [BoxGroup("切片基本信息")]
        [LabelText("启用")]
        [ShowInInspector]
        public bool Enable
        {
            get { return DiskData.enable; }
            set
            {
                DiskData.enable = value;
                if (DiskData.enable)
                {
                    View.style.borderBottomColor = new StyleColor(Color.green);
                }
                else
                {
                    View.style.borderBottomColor = new StyleColor(Color.red);
                }
            }
        }

        #region 设置样式


        public StyleBackground Background
        {
            get { return SpriteView.style.backgroundImage; }
            set
            {
                SpriteView.style.backgroundImage = value;
                Align();
            }
        }

        public void SetWidth(float value)
        {
            View.style.width = new StyleLength(new Length(value * FrameLength, LengthUnit.Pixel));
            Align();
        }
        public void SetHeight(float value)
        {
            View.style.height = new StyleLength(new Length(value, LengthUnit.Pixel));
            Align();
        }

        [BoxGroup("切片基本信息")]
        [PropertyOrder(19999)]
        [LabelText("背景")]
        [ShowInInspector]
        public Sprite Background_Sprite
        {
            get { return Background.value.sprite; }
            set { Background = new StyleBackground(value); }
        }

        [BoxGroup("切片基本信息")]
        [PropertyOrder(19999)]
        [LabelText("背景")]
        [ShowInInspector]
        public Texture2D Background_texture
        {
            get { return Background.value.texture; }
            set { Background = new StyleBackground(value); }
        }
        /// <summary>
        /// 将内部的精灵图等比例缩放
        /// </summary>
        private void Align()
        {
            if (Background.value == null) return;
            float surplusWidth = View.style.width.value.value - 2 * margin;
            float surplusHeight = View.style.height.value.value - 2 * margin;
            float wh = 0;
            if (Background.value.texture != null) wh = (float)Background.value.texture.width / Background.value.texture.height;
            else if (Background.value.sprite != null) wh = Background.value.sprite.rect.width / Background.value.sprite.rect.height;
            else return;
            if (wh > surplusWidth / surplusHeight)
            {
                SpriteView.style.width = new StyleLength(new Length(surplusWidth, LengthUnit.Pixel));
                SpriteView.style.height = new StyleLength(new Length(surplusWidth / wh, LengthUnit.Pixel));
                float m = (View.style.height.value.value - surplusWidth / wh) / 2;
                SpriteView.style.marginTop = new StyleLength(new Length(m, LengthUnit.Pixel));
                SpriteView.style.marginBottom = new StyleLength(new Length(m, LengthUnit.Pixel));
                SpriteView.style.marginRight = new StyleLength(new Length(margin, LengthUnit.Pixel));
                SpriteView.style.marginLeft = new StyleLength(new Length(margin, LengthUnit.Pixel));
            }
            else
            {
                SpriteView.style.height = new StyleLength(new Length(surplusHeight, LengthUnit.Pixel));
                SpriteView.style.width = new StyleLength(new Length(surplusHeight * wh, LengthUnit.Pixel));
                float m = (View.style.width.value.value - surplusHeight * wh) / 2;
                SpriteView.style.marginTop = new StyleLength(new Length(margin, LengthUnit.Pixel));
                SpriteView.style.marginBottom = new StyleLength(new Length(margin, LengthUnit.Pixel));
                SpriteView.style.marginRight = new StyleLength(new Length(m, LengthUnit.Pixel));
                SpriteView.style.marginLeft = new StyleLength(new Length(m, LengthUnit.Pixel));
            }
        }
        #endregion


        #region 选择与取消选择
        public virtual void Select()
        {
            SkillEditorInspector.SetShowSlice(this);
            var color = new StyleColor(Color.yellow);
            View.style.borderTopColor = color;
            View.style.borderLeftColor = color;
            View.style.borderRightColor = color;
        }
        public virtual void UnSelect()
        {
            SkillEditorInspector.SetShowSlice(null);
            var color = new StyleColor(Color.clear);
            View.style.borderTopColor = color;
            View.style.borderLeftColor = color;
            View.style.borderRightColor = color;
        }
        #endregion
        [BoxGroup("切片基本信息")]
        [PropertyOrder(20001)]
        [Button("删除")]
        public void Delete()
        {
            Track.RemoveSlice(StartFrame);
            SkillEditorWindow.Inst.SelectSlice = null;
        }

        public abstract void DrawGizmo();
    }
}
