using System;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    public class Console
    {
        public VisualElement View { get; private set; }
        public Console(VisualElement view)
        {
            View = view;
            LeftFrameButton = View.Q<Button>("LeftFrameButton");
            RightFrameButton = View.Q<Button>("RightFrameButton");
            PlayFrameButton = View.Q<Button>("PlayFrameButton");
            NowFrameField = View.Q<IntegerField>("NowFrameField");
            MaxFrameField = View.Q<IntegerField>("MaxFrameField");
            QAQ = View.Q<Label>("QAQ");


           // SkillEditorWindow.Inst.TimeScaleAxis.MaxFrame = Math.Abs(MaxFrameField.value);
            MaxFrameField.RegisterCallback<BlurEvent>(evt =>
            {
                int value = Math.Abs(MaxFrameField.value);
                SkillEditorWindow.Inst.TimeScaleAxis.MaxFrame = value;
                MaxFrameField.value = value;

            });
            NowFrameField.RegisterCallback<BlurEvent>(evt =>
            {
                int value = Math.Abs(NowFrameField.value);
                SkillEditorWindow.Inst.TimeScaleAxis.NowFrame = value;
                SkillEditorWindow.Inst.TimeScaleAxis.Align();
            });

            LeftFrameButton.clicked += () =>
            {
                SkillEditorWindow.Inst.TimeScaleAxis.NowFrame--;                
                SkillEditorWindow.Inst.TimeScaleAxis.Align();
            };
            RightFrameButton.clicked += () =>
            {
                SkillEditorWindow.Inst.TimeScaleAxis.NowFrame++;
                SkillEditorWindow.Inst.TimeScaleAxis.Align();
            };

            PlayFrameButton.clicked += () =>
            {
                SkillEditorWindow.Inst.IsUpdate = !SkillEditorWindow.Inst.IsUpdate;
            };
        }
        /// <summary>
        /// 设置当前帧文本
        /// </summary>
        /// <param name="value"></param>
        public void SetNowFrameText(int value)
        {
            NowFrameField.value = value;
        }
        public void SetMaxFrameText(int value)
        {
            MaxFrameField.value = value;
        }

        private Button LeftFrameButton;
        private Button RightFrameButton;
        private Button PlayFrameButton;

        private IntegerField NowFrameField;
        private IntegerField MaxFrameField;
        private Label QAQ;

        private int index = 0;
        public void FrameUpdate()
        {
            switch (index % 5)
            {
                default: QAQ.text = "QAQ"; break;
                case 0: QAQ.text = "(●'◡'●)"; break;
                case 1: QAQ.text = "╰(*°▽°*)╯"; break;
                case 2: QAQ.text = "(^///^)"; break;
                case 3: QAQ.text = "(⌐■_■)"; break;
                case 4: QAQ.text = "(•_•)"; break;
            }
            index++;
        }
    }
}
