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
                SkillEditorWindow.Inst.TrackParent.style.marginLeft = new StyleLength(new Length(spacing / 2, LengthUnit.Pixel));
                //���Ĺ���Ŀ�
                SkillEditorWindow.Inst.TrackList.ForEach(track => track.SetWidth(spacing));

                //����ʱ������ߵ�����߿����ڶ�����̶���
                SkillEditorWindow.Inst.TimeLine.View.style.paddingLeft = new StyleLength(new Length(spacing / 2 - 1, LengthUnit.Pixel));


                SkillEditorWindow.Inst.TrackParent.style.width = new StyleLength(new Length(MaxFrame * Spacing, LengthUnit.Pixel));
            }
        }

        private int nowFrame = 0;
        public int NowFrame
        {
            get
            {
                if (SkillEditorWindow.Inst.Config.Agent != null)
                    return SkillEditorWindow.Inst.Config.Agent.NowFrame;
                else
                    return nowFrame;
            }
            set
            {
                if (value == nowFrame) return;
                nowFrame = value;
                if (SkillEditorWindow.Inst.Config.Agent != null)
                    SkillEditorWindow.Inst.Config.Agent.NowFrame = value;
                SkillEditorWindow.Inst.Console.FrameUpdate();
                SkillEditorWindow.Inst.UpdateFrame();
                SkillEditorWindow.Inst.Console.SetNowFrameText(NowFrame);
            }
        }
        /// <summary>
        /// ��ָ����뵽�̶���
        /// </summary>
        public void Align()
        {
            SkillEditorWindow.Inst.TimeLine.Offset = Spacing * NowFrame;
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

                SkillEditorWindow.Inst.TrackParent.style.width = new StyleLength(new Length(value * Spacing, LengthUnit.Pixel));
                SkillEditorWindow.Inst.Config.Skill_Disk.MaxFrame = value;

                maxFrame = value;

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
            SkillEditorWindow.Inst.TimeLine.Offset = evt.localPosition.x - Spacing / 2;
            //����ָ��ƫ�Ƶõ�֡
            NowFrame = (int)(SkillEditorWindow.Inst.TimeLine.Offset / Spacing);
            Align();
        }

        private void InitMouseEvent()
        {

            View.RegisterCallback<PointerDownEvent>(MouseDown_TimeAxis);
            SkillEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerDownEvent>(MouseDown_Root);
            SkillEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerMoveEvent>(MouseMove_Root);
            SkillEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerLeaveEvent>(MouseLeave_Root);
            SkillEditorWindow.Inst.rootVisualElement.RegisterCallback<PointerUpEvent>(MouseUp_Root);
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
            float padding = SkillEditorWindow.Inst.TimeLine.Offset;
            SkillEditorWindow.Inst.TimeLine.Offset = padding + x;
            xOffset = NewMouseDownPosition.x;

            NowFrame = (int)(SkillEditorWindow.Inst.TimeLine.Offset / Spacing);
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
