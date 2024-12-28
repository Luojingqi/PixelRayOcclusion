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


        public SkillEditorWindowConfig Config;

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
                Config.Skill_Disk = evt.newValue as Skill_Disk;
                if (evt.newValue == null) return;
                LoadFromDisk();
            });
            var agentField = root.Q<ObjectField>("AgentField");
            agentField.objectType = typeof(SkillPlayAgent);
            agentField.RegisterValueChangedCallback(evt =>
            {
                Config.Agent = evt.newValue as SkillPlayAgent;
            });

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

            configField.value = Config.Skill_Disk;
            agentField.value = Config.Agent;
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.InSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(GameObject obj, GizmoType gizmoType)
        {
            try { if (Inst != null && Inst.Config.Agent != null) Inst.SelectSlice?.DrawGizmo(Inst.Config.Agent); }
            catch { if (Inst != null) Inst.selectSlice = null; }
        }
        private void DrawHandle(SceneView sceneView)
        {
            try { if (Inst != null && Inst.Config.Agent != null) Inst.SelectSlice?.DrawHandle(Inst.Config.Agent); }
            catch { if (Inst != null) Inst.selectSlice = null; }
        }
        private void OnEnable() => SceneView.duringSceneGui += DrawHandle;
        private void OnDestroy() => SceneView.duringSceneGui -= DrawHandle;


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
            PlayFrame();
        }

        #region 播放
        private bool isPlay = false;
        public bool IsUpdate
        {
            set
            {
                if (Config.Agent == null){ Debug.Log("请添加执行人");return; }
                nowFrameTime = TimeLine.Offset % TimeScaleAxis.Spacing / TimeScaleAxis.Spacing * Config.Skill_Disk.FrameTime;
                lastTime = DateTime.Now;
                isPlay = value;
                UpdateFrame();
            }
            get { return isPlay; }
        }


        private double nowFrameTime = 0;
        private DateTime lastTime;
        private void PlayFrame()
        {
            if (IsUpdate == false) return;
            nowFrameTime += (DateTime.Now - lastTime).TotalMilliseconds;
            if (nowFrameTime >= Config.Skill_Disk.FrameTime)
            {
                if (TimeScaleAxis.NowFrame + 1 >= TimeScaleAxis.MaxFrame)
                {
                    isPlay = false;
                    return;
                }
                nowFrameTime -= Config.Skill_Disk.FrameTime;
                TimeScaleAxis.NowFrame++;
                Console.SetNowFrameText(TimeScaleAxis.NowFrame);
            }
            TimeScaleAxis.Align();
            TimeLine.Offset += (float)(nowFrameTime / Config.Skill_Disk.FrameTime * TimeScaleAxis.Spacing);
            lastTime = DateTime.Now;
        }

        public void UpdateFrame()
        {
            Config.Skill_Disk?.UpdateFrame(Config.Agent, TimeScaleAxis.NowFrame);
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

        #region 加载技能文件

        public void LoadFromDisk()
        {
            TimeScaleAxis.MaxFrame = Config.Skill_Disk.MaxFrame;
            Console.SetMaxFrameText(TimeScaleAxis.MaxFrame);
            int index = 0;
            Config.Skill_Disk.AnimationTrack2DList.ForEach(disk => AddTrack(new AnimationTrack2D(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.SpecialEffectTrack2DList.ForEach(disk => AddTrack(new SpecialEffectTrack2D(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.ParticleTrackList.ForEach(disk => AddTrack(new ParticleTrack(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.AttackTestTrack2DList.ForEach(disk => AddTrack(new AttackTestTrack2D(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.SceneRuinTrackList.ForEach(disk => AddTrack(new SceneRuinTrack(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.EventTrackList.ForEach(disk => AddTrack(new EventTrack(disk) { trackIndex = index++ })); index = 0;
        }
        #endregion
    }
}
