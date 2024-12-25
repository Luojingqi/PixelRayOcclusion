using PRO.DataStructure;
using PRO.Tool;
using UnityEngine;
using UnityEngine.UI;

namespace PRO.SceneEditor
{
    internal class SceneEditorCanvasC : UIControllerBase
    {
        public override UIViewBase View => view;
        private SceneEditorCanvasV view = new SceneEditorCanvasV();

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

        private Element_Disk HoldEntity;
        public void SwitchHold(ElementC elementC)
        {
            view.HoldIcon.gameObject.SetActive(true);
            if (view.HoldIcon.sprite != null) GameObject.Destroy(view.HoldIcon.sprite);

            view.HoldIcon.sprite = DrawTool.CreateSprite((Texture2D)elementC.icon.texture);
            // Graphics.CopyTexture(, view.HoldIcon.sprite.texture);
            //view.HoldIcon.sprite.texture.LoadImage(elementC.icon.texture.GetNativeTexturePtr()) = elementC.icon.texture;
            HoldEntity = elementC.entity;
            //Sprite sprite;
            //sprite.texture.Load
            // view.HoldIcon.rectTransform.sizeDelta = new Vector2(HoldEntity.width, HoldEntity.height) * Camera.main.orthographicSize * 2 / Screen.height * Pixel.Size * 100;
        }
        private void ClearHold()
        {
            view.HoldIcon.gameObject.SetActive(false);
            //view.HoldIcon.texture = null;
            HoldEntity = null;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                    {
                        Block block = SceneManager.Inst.NowScene.GetBlock(new(x, y));
                        Debug.Log(block.BlockPos + "||" + block.lightSourceDic.Count);
                    }

            }

            if (HoldEntity == null) return;
            if (Input.GetKeyDown(KeyCode.Mouse1)) { ClearHold(); return; }
            Vector3 m = Input.mousePosition;
            m.z = 1;
            m = Camera.main.ScreenToWorldPoint(m);
            Vector2Int blockPos = Block.WorldToBlock(m);
            Vector2Int gloabPos = Block.WorldToGloab(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);
            view.HoldIcon.transform.position = Block.GloabToWorld(gloabPos);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                for (int y = 0; y < HoldEntity.height; y++)
                    for (int x = 0; x < HoldEntity.width; x++)
                    {
                        string typeName = HoldEntity.pixels[y * HoldEntity.width + x].typeName;
                        string colorName = HoldEntity.pixels[y * HoldEntity.width + x].colorName;
                        if (typeName == "¿ÕÆø") continue;
                        Vector2Int nowGloab = gloabPos + new Vector2Int(x, y);
                        Block block = SceneManager.Inst.NowScene.GetBlock(Block.GloabToBlock(nowGloab));
                        block.SetPixel(Pixel.TakeOut(typeName, colorName, block.GloabToPixel(nowGloab)));
                        block.DrawPixelAsync();
                    }
            }
        }
    }
}