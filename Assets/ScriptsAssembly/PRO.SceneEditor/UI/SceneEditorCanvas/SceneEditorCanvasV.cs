using UnityEngine;

namespace PRO.SceneEditor
{
    internal class SceneEditorCanvasV : UIViewBase
    {
        public FileDirectoryInfoTreeC FileDirectoryInfoTree { get; private set; }
        public ElementViewPanelC ElementViewPanel { get; private set; }
        public override void Init(Transform transform)
        {
            base.Init(transform);

            FileDirectoryInfoTree = transform.Find("FileDirectoryInfoTree").GetComponent<FileDirectoryInfoTreeC>();
            ElementViewPanel = transform.Find("ElementViewPanel").GetComponent<ElementViewPanelC>();

            ElementViewPanel.Init();
            FileDirectoryInfoTree.Init();
            

        }
    }
}