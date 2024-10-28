using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace PRO.SceneEditor
{
    internal class FileDirectoryInfoListC : MonoBehaviour
    {
        public void Init()
        {

        }

        public void ShowDirectories(DirectoryInfo[] directories, DirectoryInfo mark = null)
        {
            for (int i = 0; i < directories.Length; i++)
            {
                FileDirectoryInfoC infoC = FileDirectoryInfoTreeC.Inst.TakeOutInfo();
                infoList.Add(infoC);
                infoC.transform.parent = transform;
                infoC.SetDirectoryInfo(directories[i]);
                if (directories[i].FullName == mark?.FullName)
                    infoC.Mark();
            }
        }
        private List<FileDirectoryInfoC> infoList = new List<FileDirectoryInfoC>();
        public void Clear()
        {
            foreach (var info in infoList)
                FileDirectoryInfoTreeC.Inst.PutIn(info);
            infoList.Clear();
        }
    }
}