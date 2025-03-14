using PRO.Tool;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PRO.SceneEditor
{
    public class FileDirectoryInfoTreeC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private FileDirectoryInfoTreeV view = new FileDirectoryInfoTreeV();


        public static FileDirectoryInfoTreeC Inst { get; private set; }

        public GameObjectPool<FileDirectoryInfoListC> FileDirectoryInfoListPool;
        public GameObjectPool<FileDirectoryInfoC> FileDirectoryInfoPool;
        public override void Init()
        {
            base.Init();
            Inst = this;

            FileDirectoryInfoListPool = new GameObjectPool<FileDirectoryInfoListC>(view.FileDirectoryInfoList.gameObject, transform);
            FileDirectoryInfoPool = new GameObjectPool<FileDirectoryInfoC>(view.FileDirectoryInfo.gameObject, view.FileDirectoryInfoList.transform);

            FileDirectoryInfoListPool.CreateEventT += (g, t) => t.Init();
            FileDirectoryInfoPool.CreateEventT += (g, t) => t.Init();

            root = new DirectoryInfo($@"{Application.streamingAssetsPath}\SceneEditorData");

            SwitchDirectoryInfo(root);
        }

        public DirectoryInfo root;

        public DirectoryInfo selectDirectoryInfo;

        public List<FileDirectoryInfoListC> showDirectoryInfoList = new List<FileDirectoryInfoListC>();

        /// <summary>
        /// �����ĸ����ļ��б������������ӦΪ3�������޷�������һ����
        /// </summary>
        public static int ReserveParentListNum = 3;

        public void SwitchDirectoryInfo(DirectoryInfo info)
        {
            if (selectDirectoryInfo != null && info.FullName == selectDirectoryInfo.FullName) return;
            Clear();
            selectDirectoryInfo = info;
            ElementViewPanelC.Inst.ShowFiles(info.GetFiles());
            DirectoryInfo nowInfo = info;
            DirectoryInfo oldInfo = null;
            for (int i = 0; i < ReserveParentListNum; i++)
            {
                ShowDirectoryInfo(nowInfo, oldInfo);
                if (nowInfo.FullName != root.FullName)
                {
                    oldInfo = nowInfo;
                    nowInfo = nowInfo.Parent;
                }
                else break;
            }
            Sort();
        }
        /// <summary>
        /// չʾ��ǰ�ļ����µ������ļ�
        /// </summary>
        /// <param name="info">չʾ���ļ����µ��ļ�</param>
        /// <param name="mark">���mark�ڴ��ļ����У��ᱻ���</param>
        private void ShowDirectoryInfo(DirectoryInfo info, DirectoryInfo mark = null)
        {
            var infos = info.GetDirectories();
            if (infos.Length > 0)
            {
                FileDirectoryInfoListC infoListC = TakeOutInfoList();
                infoListC.ShowDirectories(infos, mark);
                showDirectoryInfoList.Add(infoListC);
            }
        }

        private void Sort()
        {
            for (int i = 0; i < showDirectoryInfoList.Count; i++)
                showDirectoryInfoList[i].transform.parent = null;
            for (int i = showDirectoryInfoList.Count - 1; i >= 0; i--)
                showDirectoryInfoList[i].transform.parent = transform;
        }


        private void Clear()
        {
            for (int i = 0; i < showDirectoryInfoList.Count; i++)
                PutIn(showDirectoryInfoList[i]);
            showDirectoryInfoList.Clear();
            selectDirectoryInfo = null;
        }

        private void PutIn(FileDirectoryInfoListC infoList)
        {
            infoList.Clear();
            FileDirectoryInfoListPool.PutIn(infoList.gameObject);
        }
        public void PutIn(FileDirectoryInfoC info)
        {
            info.Clear();
            FileDirectoryInfoPool.PutIn(info.gameObject);
        }

        public FileDirectoryInfoListC TakeOutInfoList()
        {
            return FileDirectoryInfoListPool.TakeOutT();
        }
        public FileDirectoryInfoC TakeOutInfo()
        {
            return FileDirectoryInfoPool.TakeOutT();
        }

    }
}