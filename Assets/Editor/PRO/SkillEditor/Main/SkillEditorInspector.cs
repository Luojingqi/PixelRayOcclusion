using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PRO.SkillEditor
{
    [CustomEditor(typeof(SkillEditorWindow))]
    internal class SkillEditorInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            if (slice == null) return new VisualElement();
            editor = OdinEditor.CreateEditor(slice);
            return editor.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            editor.OnInspectorGUI();
        }

        private static Editor editor;
        private static SliceBase slice;
        public static async void SetShowSlice(SliceBase slice)
        {
            Selection.activeObject = null;
            if (slice == null) return;
            await UniTask.Delay(75);
            SkillEditorInspector.slice = slice;
            Selection.activeObject = SkillEditorWindow.Inst;
        }
    }
}
