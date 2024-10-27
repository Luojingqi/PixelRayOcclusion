namespace PRO.SceneEditor
{
    public class ElementViewPanelC : UIControllerBase
    {
        public override UIViewBase View => view;
        private ElementViewPanelV view = new ElementViewPanelV();

        public override UIModelBase Model => model;
        private ElementViewPanelM model = new ElementViewPanelM();

        public override void Init()
        {
            base.Init();

        }
    }
}
