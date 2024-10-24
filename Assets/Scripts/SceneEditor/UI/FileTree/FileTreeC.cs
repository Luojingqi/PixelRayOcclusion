using UnityEngine;
namespace PRO
{
    internal class FileTreeC : UIChildControllerBase
    {
        public override UIChildViewBase View => view;
        private FileTreeV view = new FileTreeV();

        public override UIChildModelBase Model => model;
        private FileTreeM model = new FileTreeM();

        public override void Init()
        {
            base.Init();
        }
    }
}