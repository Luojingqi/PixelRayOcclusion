using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace PRO.SceneEditor
{
    internal class FileDirectoryInfoC : MonoBehaviour
    {
        public TMP_Text FileName_Text;
        public Button button;
        private DirectoryInfo directoryInfo;
        public void Init()
        {
            FileName_Text = transform.Find("FileName").GetComponent<TMP_Text>();
            button = transform.Find("Button").GetComponent<Button>();
        }
        public void Clear()
        {
            FileName_Text.text = "";
            directoryInfo = null;
        }
        public void SetDirectoryInfo(DirectoryInfo info)
        {

        }
        public void OnClick()
        {
            
        }
    }
}