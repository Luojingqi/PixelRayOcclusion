using PRO.DataStructure;
using PRO.Tool;
using PROTool;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PRO.SceneEditor
{
    internal class SceneEditorCanvasC : UIControllerBase
    {
        public override UIViewBase View => view;
        private SceneEditorCanvasV view = new SceneEditorCanvasV();

        public static SceneEditorCanvasC Inst;

        private List<Type> DerivedBuildingBaseList = ReflectionTool.GetDerivedClasses(typeof(BuildingBase));
        public override void Init(string uiName)
        {
            base.Init(uiName);

            Inst = this;


            for (int i = 1; i < DerivedBuildingBaseList.Count; i++)
                view.Dropdown.options.Add(new TMP_Dropdown.OptionData(DerivedBuildingBaseList[i].Name));

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
            HoldEntity = elementC.entity;
        }
        private void ClearHold()
        {
            view.HoldIcon.gameObject.SetActive(false);
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
            Vector2Int gloabPos = Block.WorldToGlobal(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);
            view.HoldIcon.transform.position = Block.GlobalToWorld(gloabPos);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                BuildingBase building = null;
                if (view.Dropdown.value != 0)
                    building = (BuildingBase)Activator.CreateInstance(DerivedBuildingBaseList[view.Dropdown.value]);


                for (int y = 0; y < HoldEntity.height; y++)
                    for (int x = 0; x < HoldEntity.width; x++)
                    {
                        string typeName = HoldEntity.pixels[y * HoldEntity.width + x].typeName;
                        string colorName = HoldEntity.pixels[y * HoldEntity.width + x].colorName;
                        if (typeName == "¿ÕÆø") continue;
                        Vector2Int nowGloab = gloabPos + new Vector2Int(x, y);
                        BlockBase block = null;
                        if (view.Toggle.isOn)
                            block = SceneManager.Inst.NowScene.GetBackground(Block.GlobalToBlock(nowGloab));
                        else
                            block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(nowGloab));

                        Pixel pixel = Pixel.TakeOut(typeName, colorName, Block.GlobalToPixel(nowGloab));
                        building?.PixelDic.Add(nowGloab, new Building_Pixel(pixel));
                        block.SetPixel(pixel);
                        block.DrawPixelAsync();
                    }
            }
        }
    }
}