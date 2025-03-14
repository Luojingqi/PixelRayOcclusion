using PRO.Tool;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace PRO.SceneEditor
{
    public class FileDirectoryInfoC : MonoBehaviour
    {
        private TMP_Text fileName_Text;
        private Button button;
        private DirectoryInfo directoryInfo;
        public void Init()
        {
            fileName_Text = transform.Find("FileName").GetComponent<TMP_Text>();
            button = transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }
        public void Clear()
        {
            fileName_Text.text = "�ѻ���";
            directoryInfo = null;
            UnMark();
        }
        public void SetDirectoryInfo(DirectoryInfo info)
        {
            fileName_Text.text = info.Name;
            directoryInfo = info;
        }
        public void OnClick()
        {
            FileDirectoryInfoTreeC.Inst.SwitchDirectoryInfo(directoryInfo);
        }

        /// <summary>
        /// ��ǣ���ʾ�ѱ�ѡ��
        /// </summary>
        public void Mark()
        {
            fileName_Text.color = Color.red;
        }

        /// <summary>
        /// ȡ�����
        /// </summary>
        public void UnMark()
        {
            fileName_Text.color = Color.black;
        }
    }
}