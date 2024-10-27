namespace PRO.SceneEditor
{
    internal class FileDirectoryInfoTreeC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private FileDirectoryInfoTreeV view = new FileDirectoryInfoTreeV();

        public override UIChildModelBase Model => model;
        private FileDirectoryInfoTreeM model = new FileDirectoryInfoTreeM();

        public override void Init()
        {
            base.Init();
        }
        internal static FileDirectoryInfoTreeC Inst;
    }
}