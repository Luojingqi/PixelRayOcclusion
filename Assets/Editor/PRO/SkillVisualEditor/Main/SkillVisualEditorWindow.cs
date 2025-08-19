using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities.UniversalDelegates;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace PRO.SkillEditor
{
    internal class SkillVisualEditorWindow : EditorWindow
    {
        public static SkillVisualEditorWindow Inst { get; private set; }


        public SkillVisualEditorWindowConfig Config;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        public VisualTreeAsset uxml_OneScaleLine;
        public VisualTreeAsset uxml_OneTrackHeading;

        [MenuItem("PRO/技能编辑器")]
        public static void ShowExample()
        {
            SkillVisualEditorWindow wnd = GetWindow<SkillVisualEditorWindow>();
            wnd.titleContent = new GUIContent("PRO技能编辑器");
        }

        public void CreateGUI()
        {
            Inst = this;
            VisualElement root = rootVisualElement;
            root.Add(m_VisualTreeAsset.Instantiate());

            var configField = root.Q<ObjectField>("ConfigField");
            configField.objectType = typeof(SkillVisual_Disk);
            configField.RegisterValueChangedCallback(evt =>
            {
                ReLoadDisk(evt.newValue as SkillVisual_Disk);
            });
            var agentField = root.Q<ObjectField>("AgentField");
            agentField.objectType = typeof(SkillPlayAgent);
            agentField.RegisterValueChangedCallback(evt =>
            {
                Config.Agent = evt.newValue as SkillPlayAgent;
            });

            var saveButton = root.Q<Button>("SaveButton");
            saveButton.clicked += Save;
            var reloadButton = root.Q<Button>("ReloadButton");
            reloadButton.clicked += () =>
            {
                configField.value = Config.Skill_Disk;
                if (configField.value == null) return;
                ClearTrack();
                LoadFromDisk();
            };

            TimeScaleAxis = new TimeScaleAxis(root.Q<VisualElement>("TimeScaleAxis"));
            TrackParent = root.Q<VisualElement>("TrackView");
            TrackHeadingParent = root.Q<VisualElement>("TrackHeadingParent");
            TimeLine = new TimeLine(root.Q<VisualElement>("TimeLine"));
            TimeScaleAxis.Spacing = 80;
            Console = new Console(root.Q<VisualElement>("Console"));
            Gizmos.color = Color.white;
            root.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == 1 && evt.ctrlKey == false) ClearSelectSlices();
            });

            agentField.value = Config.Agent;
            configField.value = Config.Skill_Disk;
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.InSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(GameObject obj, GizmoType gizmoType)
        {
            try { if (Inst != null && Inst.Config.Agent != null) foreach (var slice in Inst.SelectSliceHash) slice.DrawGizmo(Inst.Config.Agent); }
            catch { if (Inst != null) Selection.objects = null; }
        }
        private void DrawHandle(SceneView sceneView)
        {
            try { if (Inst != null && Inst.Config.Agent != null) foreach (var slice in Inst.SelectSliceHash) slice.DrawHandle(Inst.Config.Agent); }
            catch { if (Inst != null) Selection.objects = null; }
        }
        private void OnEnable()
        {
            SceneView.duringSceneGui += DrawHandle;
        }
        private void OnDestroy()
        {
            SceneView.duringSceneGui -= DrawHandle;
            Save();
        }

        public void Save()
        {

            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(Config);
                if (Config.Skill_Disk != null)
                {
                    EditorUtility.SetDirty(Config.Skill_Disk);
                }
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.Log("运行模式下不会保存，请退出运行模式后重新保存");
            }

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




        public HashSet<SliceBase> SelectSliceHash = new HashSet<SliceBase>();
        public void SwitchSelectSlice(SliceBase slice)
        {
            if (SelectSliceHash.Contains(slice))
            {
                SelectSliceHash.Remove(slice);
                slice.UnSelect();
            }
            else
            {
                SelectSliceHash.Add(slice);
                slice.Select();
            }
            Selection.objects = SelectSliceHash.ToArray();
        }

        public void ClearSelectSlices()
        {
            foreach (var slice in SelectSliceHash)
                slice.UnSelect();
            SelectSliceHash.Clear();
            Selection.objects = null;
        }

        private void UpdateValueChange()
        {
            valueChangeTime += Time.deltaTime;
            if (valueChangeTime > 1f)
            {
                valueChangeTime = 0;
                foreach (var slice in SelectSliceHash)
                    slice.ValueChangeReset();
            }
        }

        private float valueChangeTime = 0;
        public void Update()
        {
            PlayFrame();
            UpdateValueChange();
        }

        #region 播放
        private bool isPlay = false;
        public bool IsUpdate
        {
            set
            {
                if (Config.Agent == null) { Debug.Log("请添加执行人"); return; }
                if (value == true)
                {
                    TimeScaleAxis.Align();
                    playData = new SkillPlayData();
                    playData.time = TimeLine.Offset % TimeScaleAxis.Spacing / TimeScaleAxis.Spacing * Config.Skill_Disk.FrameTime;
                    playData.nowFrame = TimeScaleAxis.nowFrame;
                    lastTime = DateTime.Now;
                    playData.SkillVisual = Config.Skill_Disk;
                    Config.Agent.SkillPlayDataList.Add(playData);
                }
                isPlay = value;
            }
            get { return isPlay; }
        }


        private DateTime lastTime;
        private void PlayFrame()
        {
            if (IsUpdate == false || Config.Agent == null) return;
            

            var deltaTime = (DateTime.Now - lastTime).TotalSeconds;

            if (playData.UpdateFrameScript(Config.Agent, (float)deltaTime))
            {
                Config.Agent.SkillPlayDataList.Remove(playData);
                isPlay = false;
                TimeScaleAxis.nowFrame = 0;
                Console.FrameUpdate();
                Console.SetNowFrameText(TimeScaleAxis.nowFrame);
                TimeScaleAxis.Align();
            }
            else
            {
                if (playData.nowFrame != TimeScaleAxis.nowFrame)
                {
                    TimeScaleAxis.nowFrame = playData.nowFrame;
                    Console.FrameUpdate();
                    Console.SetNowFrameText(TimeScaleAxis.nowFrame);
                }
                TimeScaleAxis.Align();
                TimeLine.Offset += (playData.time / Config.Skill_Disk.FrameTime * TimeScaleAxis.Spacing);
                lastTime = DateTime.Now;
            }
        }

        private SkillPlayData playData;
        public void UpdateFrame()
        {
            playData.nowFrame = TimeScaleAxis.NowFrame;
            playData.SkillVisual = Config.Skill_Disk;
            Config.Skill_Disk?.UpdateFrame(Config.Agent, playData);
        }
        public void PlaySlice(Slice_DiskBase slice, int trackIndex)
        {
            if (Config.Agent == null) return;
            var playData = new SkillPlayData();
            playData.nowFrame = slice.startFrame;
            playData.SkillVisual = Config.Skill_Disk;
            slice.UpdateFrame(Config.Agent, playData, new FrameData(slice.frameLength, 0, trackIndex));
        }
        public void PlaySlice(SliceBase slice)
        {
            PlaySlice(slice.DiskData, slice.Track.trackIndex);
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
        public void ReLoadDisk(SkillVisual_Disk newSkillVisual)
        {
            Save();
            ClearTrack();
            Config.Skill_Disk = newSkillVisual;
            if (newSkillVisual == null) return;
            LoadFromDisk();
        }
        public void LoadFromDisk()
        {
            int index = 0;
            Config.Skill_Disk.AnimationTrack2DList.ForEach(disk => AddTrack(new AnimationTrack2D(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.SpecialEffectTrack2DList.ForEach(disk => AddTrack(new SpecialEffectTrack2D(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.ParticleTrackList.ForEach(disk => AddTrack(new ParticleTrack(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.AttackTestTrack2DList.ForEach(disk => AddTrack(new AttackTestTrack2D(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.SceneRuinTrackList.ForEach(disk => AddTrack(new SceneRuinTrack(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.SceneCreateTrackList.ForEach(disk => AddTrack(new SceneCreateTrack(disk) { trackIndex = index++ })); index = 0;
            Config.Skill_Disk.EventTrackList.ForEach(disk => AddTrack(new EventTrack(disk) { trackIndex = index++ })); index = 0;

            TimeScaleAxis.MaxFrame = Config.Skill_Disk.MaxFrame;
            Console.SetMaxFrameText(TimeScaleAxis.MaxFrame);
        }
        #endregion
    }
}
