using UnityEditor;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class OneTrackHeading
    {
        #region UXML
        private static VisualTreeAsset uxml
        {
            get
            {
                if (_uxml != null) return _uxml;
                else { _uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath); return _uxml; }
            }
        }
        private static VisualTreeAsset _uxml;
        private static string uxmlPath = "Assets/Editor/PRO/SkillEditor/Track/TrackBase/TrackHeading.uxml";
        #endregion
        public VisualElement View { get; private set; }
        public OneTrackHeading(TrackBase track)
        {
            Track = track;
            View = new VisualElement();
            uxml.CloneTree(View);
            View = View.Q<VisualElement>("OneTrackHeading");
            NameText = View.Q<Label>("Name");

            View.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("删除轨道", _ => { });
            }));
        }
        public TrackBase Track { get; private set; }
        public Label NameText { get; private set; }
    }
}
