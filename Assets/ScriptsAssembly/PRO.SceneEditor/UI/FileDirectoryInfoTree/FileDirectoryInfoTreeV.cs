using UnityEngine;

namespace PRO.SceneEditor
{
    internal class FileDirectoryInfoTreeV : UIChildViewBase
    {
        public FileDirectoryInfoListC FileDirectoryInfoList { get; private set; }
        public FileDirectoryInfoC FileDirectoryInfo { get; private set; }
        public override void Init(Transform transform)
        {
            base.Init(transform);

            FileDirectoryInfoList = transform.Find("FileDirectoryInfoList").GetComponent<FileDirectoryInfoListC>();
            FileDirectoryInfo = transform.Find("FileDirectoryInfoList/FileDirectoryInfo").GetComponent<FileDirectoryInfoC>();
        }
    }
}