namespace PRO.SceneEditor
{
    internal class SceneEditorCanvasC : UIControllerBase
    {
        public override UIViewBase View => view;
        private SceneEditorCanvasV view = new SceneEditorCanvasV();
        public override UIModelBase Model => model;
        private SceneEditorCanvasM model = new SceneEditorCanvasM();

        public static SceneEditorCanvasC Inst;

        public override void Init(string uiName)
        {
            base.Init(uiName);

            Inst = this;
            //Scene
        }

        public void Start()
        {
            Init("123");
        }
    }
}