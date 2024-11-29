using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal class SkillEditorWindow : EditorWindow
    {
        public static SkillEditorWindow Inst { get; private set; }

        public SkillPlayAgent Agent;

        public Skill_Disk Skill_Disk;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("PRO/技能编辑器")]
        public static void ShowExample()
        {
            SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
            wnd.titleContent = new GUIContent("PRO技能编辑器");
        }

        public void CreateGUI()
        {
            Inst = this;
            VisualElement root = rootVisualElement;
            root.Add(m_VisualTreeAsset.Instantiate());

            var configField = root.Q<ObjectField>("ConfigField");
            configField.objectType = typeof(Skill_Disk);
            configField.RegisterValueChangedCallback(evt =>
            {
                ClearTrack();
                Skill_Disk = evt.newValue as Skill_Disk;
                if (evt.newValue == null) return;
                LoadFromDisk();
            });
            var agentField = root.Q<ObjectField>("AgentField");
            agentField.objectType = typeof(SkillPlayAgent);
            agentField.RegisterValueChangedCallback(evt =>
            {
                Agent = evt.newValue as SkillPlayAgent;
            });


            // 路径 Down/Right/TimeScaleAxis
            TimeScaleAxis = new TimeScaleAxis(root.Q<VisualElement>("TimeScaleAxis"));
            TrackParent = root.Q<VisualElement>("TrackView");
            TrackHeadingParent = root.Q<VisualElement>("TrackHeadingParent");
            TimeLine = new TimeLine(root.Q<VisualElement>("TimeLine"));
            TimeScaleAxis.Spacing = 80;
            Console = new Console(root.Q<VisualElement>("Console"));
            Gizmos.color = Color.white;
            root.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1) SkillEditorWindow.Inst.SelectSlice = null;
            });
            //root.AddManipulator(new ContextualMenuManipulator(evt =>
            //{
            //    evt.menu.AppendAction("Action 4", _ => Debug.Log("Action 1 triggered"));
            //    evt.menu.AppendAction("Action 5", _ => Debug.Log("Action 2 triggered"));
            //    evt.menu.AppendSeparator();
            //    evt.menu.AppendAction("Action 6", _ => Debug.Log("Action 3 triggered"), DropdownMenuAction.Status.Disabled);
            //}));


        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.InSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        public static void DrawGizmo(GameObject obj, GizmoType gizmoType)
        {
            try { SkillEditorWindow.Inst?.SelectSlice?.DrawGizmo(); }
            catch { if (SkillEditorWindow.Inst != null) SkillEditorWindow.Inst.selectSlice = null; }
        }



        /// <summary>
        /// 时间刻度轴
        /// </summary>
        public TimeScaleAxis TimeScaleAxis { get; private set; }
        /// <summary>
        /// 时间控制线
        /// </summary>
        public TimeLine TimeLine { get; private set; }
        /// <summary>
        /// 控制台
        /// </summary>
        public Console Console { get; private set; }

        /// <summary>
        /// 轨道的父物体
        /// </summary>
        public VisualElement TrackParent { get; private set; }
        /// <summary>
        /// 轨道表头的父物体
        /// </summary>
        public VisualElement TrackHeadingParent { get; private set; }





        /// <summary>
        /// 选择的切片，触发切片的选择事件（Inspector显示数据等）
        /// </summary>
        public SliceBase SelectSlice
        {
            get { return selectSlice; }
            set
            {
                selectSlice?.UnSelect();
                selectSlice = value;
                selectSlice?.Select();
            }
        }
        private SliceBase selectSlice;



        public void Update()
        {
            UpdateFrame();
        }

        #region 播放
        private bool isUpdate = false;
        public bool IsUpdate
        {
            set
            {
                nowFrameTime = TimeLine.Offset % TimeScaleAxis.Spacing / TimeScaleAxis.Spacing * Skill_Disk.FrameTime;
                lastTime = DateTime.Now;
                isUpdate = value;
                Skill_Disk?.UpdateFrame(Agent, TimeScaleAxis.NowFrame);
            }
            get { return isUpdate; }
        }


        private double nowFrameTime = 0;
        private DateTime lastTime;
        private void UpdateFrame()
        {
            if (IsUpdate == false) return;
            nowFrameTime += (DateTime.Now - lastTime).TotalMilliseconds;
            if (nowFrameTime >= Skill_Disk.FrameTime)
            {
                if (TimeScaleAxis.NowFrame + 1 >= TimeScaleAxis.MaxFrame)
                {
                    isUpdate = false;
                    return;
                }
                nowFrameTime -= Skill_Disk.FrameTime;
                TimeScaleAxis.NowFrame++;
                Console.SetNowFrameText(TimeScaleAxis.NowFrame);
            }
            TimeScaleAxis.Align();
            TimeLine.Offset += (float)(nowFrameTime / Skill_Disk.FrameTime * TimeScaleAxis.Spacing);
            lastTime = DateTime.Now;
        }
        #endregion


        #region 添加轨道
        public List<TrackBase> TrackList = new List<TrackBase>();

        public void AddTrack(TrackBase trackBase)
        {
            TrackList.Add(trackBase);
            TrackParent.Add(trackBase.View);
            TrackHeadingParent.Add(trackBase.Heading.View);
        }

        public void ClearTrack()
        {
            TrackList.Clear();
            TrackParent.Clear();
            TrackHeadingParent.Clear();
        }

        #endregion

        #region 保存与加载
        public void SaveToDisk()
        {
            ///弃用
            if (Skill_Disk == null) return;
            this.Skill_Disk.Clear();
            Skill_Disk.MaxFrame = TimeScaleAxis.MaxFrame;

            foreach (var track in TrackList)
            {
                Track_Disk track_Disk = new Track_Disk();
                foreach (var slick in track.sliceList)
                    track_Disk.SlickList.Add(slick.DiskData);
                switch (track)
                {
                    case AnimationTrack2D: Skill_Disk.AnimationTrack2DList.Add(track_Disk); break;
                    case AttackTestTrack2D: Skill_Disk.AttackTestTrack2DList.Add(track_Disk); break;
                    case EventTrack: Skill_Disk.EventTrackList.Add(track_Disk); break;
                }
            }
        }

        public void LoadFromDisk()
        {
            TimeScaleAxis.MaxFrame = Skill_Disk.MaxFrame;

            Skill_Disk.AnimationTrack2DList.ForEach(disk => AddTrack(new AnimationTrack2D(disk)));
            Skill_Disk.AttackTestTrack2DList.ForEach(disk => AddTrack(new AttackTestTrack2D(disk)));
            Skill_Disk.SceneRuinTrackList.ForEach(disk => AddTrack(new SceneRuinTrack(disk)));
            Skill_Disk.EventTrackList.ForEach(disk => AddTrack(new EventTrack(disk)));
        }
        #endregion
    }
}
