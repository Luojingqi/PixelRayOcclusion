using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    internal class OneTrackHeading
    {
        public VisualElement View { get; private set; }
        public OneTrackHeading(TrackBase track)
        {
            Track = track;
            View = new VisualElement();
            SkillVisualEditorWindow.Inst.uxml_OneTrackHeading.CloneTree(View);
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
