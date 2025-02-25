using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    /// <summary>
    /// 事件刻度轴的面板
    /// </summary>
    internal class TimeScaleAxis
    {

        public VisualElement View { get; private set; }
        public TimeScaleAxis(VisualElement view)
        {
            this.View = view;
            InitMouseEvent();
            #region 添加0刻度线
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
        /// 刻度线间距，会更新所有相关ui的间距
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
                //轨道视图的左边距，用于将所有轨道右移然后对齐0刻度线
                SkillEditorWindow.Inst.TrackParent.style.marginLeft = new StyleLength(new Length(spacing / 2, LengthUnit.Pixel));
                //更改轨道的宽
                SkillEditorWindow.Inst.TrackList.ForEach(track => track.SetWidth(spacing));

                //更改时间控制线的左外边框，用于对齐零刻度线
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
        /// 将指针对齐到刻度线
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
                        //每5个刻度线显示刻度字
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


        #region 鼠标事件
        /// <summary>
        /// 鼠标双击，指针指到当前帧的开始
        /// </summary>
        /// <param name="evt">123</param>
        private void MouseTwoDown(PointerDownEvent evt)
        {
            //设置指针偏移
            SkillEditorWindow.Inst.TimeLine.Offset = evt.localPosition.x - Spacing / 2;
            //根据指针偏移得到帧
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
