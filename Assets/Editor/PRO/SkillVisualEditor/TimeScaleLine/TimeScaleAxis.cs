using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    /// <summary>
    /// �¼��̶�������
    /// </summary>
    internal class TimeScaleAxis
    {

        public VisualElement View { get; private set; }
        public TimeScaleAxis(VisualElement view)
        {
            this.View = view;
            InitMouseEvent();
            #region ���0�̶���
            OneScaleLine scaleLine = new OneScaleLine();
            View.Add(scaleLine.View);
            scaleLine.SetScaleLine(OneScaleLine.ScaleLineType.l);
            scaleLine.SetText("0");
            scaleLineList.Add(scaleLine);
            #endregion

            View.RegisterCallback<WheelEvent>(evt =>
            {
                if (evt.mouseDelta.y > 0)
                {
                    Spacing -= 3;
                }
                else if (evt.mouseDelta.y < 0)
                {
                    Spacing += 3;
                }
            });
        }

        private List<OneScaleLine> scaleLineList = new List<OneScaleLine>();

        private int spacing = 0;
        /// <summary>
        /// �̶��߼�࣬������������ui�ļ��
        /// </summary>
        public int Spacing
        {
            get { return spacing; }
            set
            {
                spacing = value;
                foreach (var line in scaleLineList)
                {
                    line.SetWidth(spacing);
                }
                //�����ͼ����߾࣬���ڽ����й������Ȼ�����0�̶���
                SkillVisualEditorWindow.Inst.TrackParent.style.marginLeft = new StyleLength(new Length(spacing / 2, LengthUnit.Pixel));
                //���Ĺ���Ŀ�
                SkillVisualEditorWindow.Inst.TrackList.ForEach(track =>
                {
                    track.SetWidth(spacing);
                    track.Reset();
                });

                //����ʱ������ߵ�����߿����ڶ�����̶���
                SkillVisualEditorWindow.Inst.TimeLine.View.style.paddingLeft = new StyleLength(new Length(spacing / 2 - 1, LengthUnit.Pixel));


                SkillVisualEditorWindow.Inst.TrackParent.style.width = new StyleLength(new Length(MaxFrame * Spacing, LengthUnit.Pixel));
            }
        }

        public int nowFrame = 0;
        public int NowFrame
        {
            get
            {
                return nowFrame;
            }
            set
            {
                if (value == nowFrame) return;
                if (value < 0) return;
                if (value >= SkillVisualEditorWindow.Inst.Config.Skill_Disk.MaxFrame) return;

                nowFrame = value;
                SkillVisualEditorWindow.Inst.Console.FrameUpdate();
                SkillVisualEditorWindow.Inst.UpdateFrame();
                SkillVisualEditorWindow.Inst.Console.SetNowFrameText(NowFrame);
            }
        }
        /// <summary>
        /// ��ָ����뵽�̶���
        /// </summary>
        public void Align()
        {
            SkillVisualEditorWindow.Inst.TimeLine.Offset = Spacing * NowFrame;
        }

        private int maxFrame = 0;
        public int MaxFrame
        {
            get
            {
                return maxFrame;
            }
            set
            {
                if (value <= 0) return;
                if (maxFrame == value) return;
                if (maxFrame > value)
                {
                    for (int i = maxFrame; i > value; i--)
                    {
                        scaleLineList[i].View.RemoveFromHierarchy();
                        scaleLineList.RemoveAt(i);
                    }
                }
                else
                {
                    for (int i = maxFrame + 1; i <= value; i++)
                    {
                        OneScaleLine scaleLine = new OneScaleLine();
                        View.Add(scaleLine.View);
                        //ÿ5���̶�����ʾ�̶���
                        if (i % 5 == 0)
                        {
                            scaleLine.SetText(i.ToString());
                            scaleLine.SetScaleLine(OneScaleLine.ScaleLineType.l);
                        }
                        else
                        {
                            scaleLine.SetText(null);
                            scaleLine.SetScaleLine(OneScaleLine.ScaleLineType.s);
                        }
                        scaleLine.SetWidth(Spacing);
                        scaleLineList.Add(scaleLine);
                    }
                }

                SkillVisualEditorWindow.Inst.TrackParent.style.width = new StyleLength(new Length(value * Spacing, LengthUnit.Pixel));
                SkillVisualEditorWindow.Inst.Config.Skill_Disk.SetMaxFrame(value);

                maxFrame = value;
                SkillVisualEditorWindow.Inst.ReLoadDisk(SkillVisualEditorWindow.Inst.Config.Skill_Disk);
            }
        }


        #region ����¼�
        /// <summary>
        /// ���˫����ָ��ָ����ǰ֡�Ŀ�ʼ
        /// </summary>
        /// <param name="evt">123</param>
        private void MouseTwoDown(PointerDownEvent evt)
        {
            //����ָ��ƫ��
            SkillVisualEditorWindow.Inst.TimeLine.Offset = evt.localPosition.x - Spacing / 2;
            //����ָ��ƫ�Ƶõ�֡
            NowFrame = (int)(SkillVisualEditorWindow.Inst.TimeLine.Offset / Spacing);
            Align();
        }

        private void InitMouseEvent()
        {

            View.RegisterCallback<PointerDownEvent>(MouseDown_TimeAxis);
            SkillVisualEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerDownEvent>(MouseDown_Root);
            SkillVisualEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerMoveEvent>(MouseMove_Root);
            SkillVisualEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerLeaveEvent>(MouseLeave_Root);
            SkillVisualEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerUpEvent>(MouseUp_Root);
        }


        private bool IsMouseDown = false;

        private DateTime mouseDownTimeScale = DateTime.MinValue;
        private void MouseDown_TimeAxis(PointerDownEvent evt)
        {
            IsMouseDown = true;
            if ((DateTime.Now - mouseDownTimeScale).TotalMilliseconds <= 500)
            {
                MouseTwoDown(evt);
                IsMouseDown = false;
            }
            mouseDownTimeScale = DateTime.Now;
        }


        private float xOffset;
        private void MouseDown_Root(PointerDownEvent evt)
        {
            xOffset = evt.localPosition.x;
        }
        private void MouseMove_Root(PointerMoveEvent evt)
        {
            if (IsMouseDown == false) return;
            Vector3 NewMouseDownPosition = evt.localPosition;
            float x = NewMouseDownPosition.x - xOffset;
            float padding = SkillVisualEditorWindow.Inst.TimeLine.Offset;
            SkillVisualEditorWindow.Inst.TimeLine.Offset = padding + x;
            xOffset = NewMouseDownPosition.x;

            NowFrame = (int)(SkillVisualEditorWindow.Inst.TimeLine.Offset / Spacing);
        }
        private void MouseLeave_Root(PointerLeaveEvent evt)
        {
            IsMouseDown = false;
        }
        private void MouseUp_Root(PointerUpEvent evt)
        {
            IsMouseDown = false;
        }
        #endregion
    }
}
