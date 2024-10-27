namespace PRO
{
    public class BackgroundBlock : BlockBase
    {
        public override void Init()
        {
            base.Init();
            spriteRenderer.sortingOrder = -1;
        }

    }
}
