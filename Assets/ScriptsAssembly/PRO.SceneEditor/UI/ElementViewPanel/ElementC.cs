using PRO.SceneEditor;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementC : MonoBehaviour
{
    public RawImage icon;
    private TMP_Text name_Text;
    private Button button;
    public Element_Disk entity;

    public void Init()
    {
        icon = transform.Find("icon").GetComponent<RawImage>();
        name_Text = transform.Find("Name").GetComponent<TMP_Text>();
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(() => { SceneEditorCanvasC.Inst.SwitchHold(this); });
    }

    public void Clear()
    {
        name_Text.text = "“—ªÿ ’";
        entity = null;
        Destroy(icon.texture);
        icon.texture = null;
    }

    public void SetEntity(FileInfo info, Element_Disk entity)
    {
        name_Text.text = entity.name;
        this.entity = entity;
        Texture2D texture2D = new Texture2D(0, 0);
        texture2D.filterMode = FilterMode.Point;
        texture2D.LoadImage(entity.pngBytes);
        icon.texture = texture2D;
    }
}
