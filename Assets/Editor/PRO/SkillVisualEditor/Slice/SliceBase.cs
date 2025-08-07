using Sirenix.OdinInspector;
using System.Reflection;
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

        protected SliceBase() { }

        public virtual void Init(Slice_DiskBase sliceDisk)
        {
            DiskData = sliceDisk;
            View = new VisualElement();
            View.name = "Slice";
            View.style.position = Position.Absolute;
            #region ��ʼ���߿�
            View.style.backgroundColor = new StyleColor(Color.gray);
            View.style.borderTopWidth = new StyleFloat(3);
            View.style.borderBottomWidth = new StyleFloat(3);
            View.style.borderLeftWidth = new StyleFloat(3);
            View.style.borderRightWidth = new StyleFloat(3);
            View.style.borderBottomLeftRadius = new StyleLength(new Length(8, LengthUnit.Pixel));
            View.style.borderBottomRightRadius = new StyleLength(new Length(8, LengthUnit.Pixel));
            #endregion
            #region ���ñ�����
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
            #region ����¼�
            View.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 0)
                {
                    if (!evt.ctrlKey)
                    {
                        SkillVisualEditorWindow.Inst.ClearSelectSlices();
                    }
                    SkillVisualEditorWindow.Inst.SwitchSelectSlice(this);
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
                evt.StopPropagation();
            });
            #endregion

            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("���� | ����", _ => Enable = !Enable);

                evt.menu.AppendAction("ɾ����Ƭ", _ => Track.RemoveSlice(StartFrame));
            }));
        }


        [PropertyOrder(20000)]
        [BoxGroup]
        [LabelText("��������")]
        [ShowInInspector]
        public Slice_DiskBase DiskData { get; set; }

        [BoxGroup("��Ƭ������Ϣ")]
        [LabelText("����")]
        [ShowInInspector]
        public string Name
        {
            get { return DiskData.name; }
            set { LabelView.text = value; DiskData.name = value; }
        }
        [ReadOnly]
        [BoxGroup("��Ƭ������Ϣ")]
        [LabelText("��ʼ֡")]
        [ShowInInspector]
        public int StartFrame
        {
            get { return DiskData.startFrame; }
            set
            {
                DiskData.startFrame = value;
                View.style.left = SkillVisualEditorWindow.Inst.TimeScaleAxis.Spacing * value;
                //Track.SwapSlice(StartFrame, value);
            }
        }
        [BoxGroup("��Ƭ������Ϣ")]
        [LabelText("֡����")]
        [ShowInInspector]
        public int FrameLength
        {
            get { return DiskData.frameLength; }
            set { DiskData.frameLength = value; Track.SetSliceFrameLength(StartFrame, value); SetWidth(SkillVisualEditorWindow.Inst.TimeScaleAxis.Spacing); }
        }
        [BoxGroup("��Ƭ������Ϣ")]
        [LabelText("����")]
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

        #region ������ʽ
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

        [BoxGroup("��Ƭ������Ϣ")]
        [LabelText("����")]
        [ShowInInspector]
        public Sprite Background_Sprite
        {
            get { return Background.value.sprite; }
            set { Background = new StyleBackground(value); }
        }

        [BoxGroup("��Ƭ������Ϣ")]
        [LabelText("����")]
        [ShowInInspector]
        public Texture2D Background_texture
        {
            get { return Background.value.texture; }
            set { Background = new StyleBackground(value); }
        }
        /// <summary>
        /// ���ڲ��ľ���ͼ�ȱ�������
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


        #region ѡ����ȡ��ѡ��
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
                Debug.Log("�ű��������ģ������´򿪼��ܱ༭��");
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
                FieldInfo field = DiskData.GetType().GetField("changeValue", BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    object fieldValue = field.GetValue(DiskData);
                    MethodInfo resetMethod = fieldValue.GetType().GetMethod("Reset");
                    resetMethod.Invoke(fieldValue,new object[] { DiskData });
                }
            }
            catch
            {
                Debug.Log("�ű��������ģ������´򿪼��ܱ༭��");
            }
        }
        #endregion
        [BoxGroup("��Ƭ������Ϣ")]
        [Button("ɾ��")]
        public void Delete()
        {
            SkillVisualEditorWindow.Inst.SwitchSelectSlice(Track.sliceArray[StartFrame]);
            Track.RemoveSlice(StartFrame);

        }
        #region �ָ��������
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
        /// ���Ƴ�������
        /// </summary>
        public abstract void DrawGizmo(SkillPlayAgent agent);
        /// <summary>
        /// ���Ƴ��������ֱ�
        /// </summary>
        public abstract void DrawHandle(SkillPlayAgent agent);


        #region һЩ�ֱ���ʵ��
        protected bool HandlePosition(Matrix4x4 parent_rts, ref Vector3 position)
        {
            if (Tools.current != UnityEditor.Tool.Move && Tools.current != UnityEditor.Tool.Transform) return false;
            Handles.matrix = parent_rts;
            EditorGUI.BeginChangeCheck();
            var ret = Handles.PositionHandle(position, Quaternion.Inverse(parent_rts.rotation));
            if (EditorGUI.EndChangeCheck())
            {
                position = ret;
                return true;
            }
            return false;
        }
        protected bool HandleRotation(Matrix4x4 parent_rts, ref Quaternion rotation)
        {
            if (Tools.current != UnityEditor.Tool.Rotate && Tools.current != UnityEditor.Tool.Transform) return false;
            Handles.matrix = parent_rts;
            EditorGUI.BeginChangeCheck();
            var ret = Handles.RotationHandle(rotation, Vector3.zero);
            if (EditorGUI.EndChangeCheck())
            {
                rotation = ret;
                return true;
            }
            return false;
        }
        protected bool HandleScale(Matrix4x4 rts, ref Vector3 scale)
        {
            if (Tools.current != UnityEditor.Tool.Scale && Tools.current != UnityEditor.Tool.Transform) return false;
            Handles.matrix = rts;
            EditorGUI.BeginChangeCheck();
            var ret = Handles.ScaleHandle(scale, Vector3.zero, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                scale = ret;
                return true;
            }
            return false;
        }
        #endregion
    }
}
