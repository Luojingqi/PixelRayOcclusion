using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementC : MonoBehaviour
{
    private Image icon;
    private TMP_Text name_Text;
    public void Init()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        name_Text = transform.Find("Name").GetComponent<TMP_Text>();
    }

    public void Clear()
    {
        name_Text.text = "“—ªÿ ’";
    }

    public void SetFileInfo(FileInfo info)
    {
        name_Text.text = info.Name;
    }
}
