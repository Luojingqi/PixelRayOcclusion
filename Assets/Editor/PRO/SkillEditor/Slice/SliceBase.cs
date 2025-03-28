using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal abstract class SliceBase : SerializedScriptableObject
    {
        private static int margin = 4;
        public VisualElement View { get; private set; }
        protected VisualElement SpriteView;
        protected Label LabelView;
        [HideInInspector]
        public TrackBase Track;
        public SliceBase(SliceBase_Disk sliceDisk)
        {
            DiskData = sliceDisk;
            View = new VisualElement();
            View.name = "Slice";
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
            View.Add(LabelView);
            LabelView.style.color = new StyleColor(Color.black);
            LabelView.style.fontSize = new StyleLength(new Length(15, LengthUnit.Pixel));
            Name = Name;
            #endregion
            SpriteView = new VisualElement();
            LabelView.Add(SpriteView);
            SpriteView.name = "Sprite";
            Enable = sliceDisk.enable;
            #region 鼠标事件
            View.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    if (!evt.ctrlKey)
                    {
                        SkillEditorWindow.Inst.ClearSelectSlices();
                    }
                    SkillEditorWindow.Inst.SwitchSelectSlice(this);
                }
                if (evt.button == 1)
                {
                    evt.StopPropagation();
                }
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
        [ReadOnly]
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
            set { SpriteView.style.backgroundImage = value; Align(); }
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
        [LabelText("背景")]
        [ShowInInspector]
        public Sprite Background_Sprite
        {
            get { return Background.value.sprite; }
            set { Background = new StyleBackground(value); }
        }

        [BoxGroup("切片基本信息")]
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
            var color = new StyleColor(Color.yellow);
            try
            {
                View.style.borderTopColor = color;
                View.style.borderLeftColor = color;
                View.style.borderRightColor = color;
            }
            catch
            {
                Debug.Log("脚本发生更改，请重新打开技能编辑器");
            }
        }
        public virtual void UnSelect()
        {
            var color = new StyleColor(Color.clear);
            try
            {
                View.style.borderTopColor = color;
                View.style.borderLeftColor = color;
                View.style.borderRightColor = color;
            }
            catch
            {
                Debug.Log("脚本发生更改，请重新打开技能编辑器");
            }
        }
        #endregion
        [BoxGroup("切片基本信息")]
        [Button("删除")]
        public void Delete()
        {
            SkillEditorWindow.Inst.SwitchSelectSlice(Track.sliceList[StartFrame]);
            Track.RemoveSlice(StartFrame);

        }
        #region 分割属性面板
        [GUIColor(0, 0, 0, 0)]
        [ShowInInspector]
        [PropertyOrder(0)]
        int Divider0 { get; }

        [GUIColor(0, 0, 0, 0)]
        [ShowInInspector]
        [PropertyOrder(19999)]
        void Divider1() { }
        #endregion
        /// <summary>
        /// 绘制场景框线
        /// </summary>
        public abstract void DrawGizmo(SkillPlayAgent agent);
        /// <summary>
        /// 绘制场景控制手柄
        /// </summary>
        public abstract void DrawHandle(SkillPlayAgent agent);


        #region 一些手柄的实现
        protected Vector3 V3mV3(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        protected Vector3 V3dV3(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

        protected void HandlePosition(SkillPlayAgent agent, Quaternion rotation, ref Vector3 position)
        {
            if (Tools.current != UnityEditor.Tool.Move) return;
            Handles.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale);
            EditorGUI.BeginChangeCheck();
            Vector3 ret = Handles.PositionHandle(position, rotation);
            if (EditorGUI.EndChangeCheck())
            {
                position = ret;
                SkillEditorWindow.Inst.UpdateFrame();
            }
        }
        protected void HandleRotation(SkillPlayAgent agent, Vector3 position, ref Quaternion rotation)
        {
            if (Tools.current != UnityEditor.Tool.Rotate) return;
            Handles.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale);
            EditorGUI.BeginChangeCheck();
            Quaternion ret = Handles.RotationHandle(rotation, position);
            if (EditorGUI.EndChangeCheck())
            {
                rotation = ret;
                SkillEditorWindow.Inst.UpdateFrame();
            }
        }
        protected void HandleScale(SkillPlayAgent agent, Vector3 position, Quaternion rotation, ref Vector3 scale)
        {
            if (Tools.current != UnityEditor.Tool.Scale) return;
            Handles.matrix = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, agent.transform.lossyScale);
            EditorGUI.BeginChangeCheck();
            Vector3 ret = Handles.ScaleHandle(scale, position, rotation);
            if (EditorGUI.EndChangeCheck())
            {
                scale = ret;
                SkillEditorWindow.Inst.UpdateFrame();
            }
        }
        #endregion
    }
}
