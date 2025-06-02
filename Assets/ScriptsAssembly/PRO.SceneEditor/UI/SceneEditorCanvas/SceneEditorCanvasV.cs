using PRO.Tool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PRO.SceneEditor
{
    public class SceneEditorCanvasV : UIViewBase
    {
        public FileDirectoryInfoTreeC FileDirectoryInfoTree { get; private set; }
        public ElementViewPanelC ElementViewPanel { get; private set; }

        public SpriteRenderer HoldIcon { get; private set; }
        public Toggle Toggle { get; private set; }
        public TMP_Dropdown Dropdown { get; private set; }
        public override void Init(UIControllerBase controller)
        {
            base.Init(controller);

            FileDirectoryInfoTree = transform.Find("FileDirectoryInfoTree").GetComponent<FileDirectoryInfoTreeC>();
            ElementViewPanel = transform.Find("ElementViewPanel").GetComponent<ElementViewPanelC>();

            HoldIcon = new GameObject("HoldIcon").AddComponent<SpriteRenderer>();
            HoldIcon.gameObject.SetActive(false);

            Toggle = transform.Find("Toggle").GetComponent<Toggle>();
            Dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        }
    }
}