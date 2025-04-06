using PRO.DataStructure;
using PRO.Tool;
using PROTool;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PRO.SceneEditor
{
    public class SceneEditorCanvasC : UIControllerBase
    {
        public override UIViewBase View => view;
        private SceneEditorCanvasV view = new SceneEditorCanvasV();

        public static SceneEditorCanvasC Inst;

        private List<Type> DerivedBuildingBaseList = ReflectionTool.GetDerivedClasses(typeof(BuildingBase));
        public override void Init(string uiName)
        {
            base.Init(uiName);

            Inst = this;


            for (int i = 0; i < DerivedBuildingBaseList.Count; i++)
                view.Dropdown.options.Add(new TMP_Dropdown.OptionData(DerivedBuildingBaseList[i].Name));
        }

        public void Start()
        {
            Init("123");
            gameObject.SetActive(false);
        }

        private Element_Disk HoldEntity;
        public void SwitchHold(ElementC elementC)
        {
            view.HoldIcon.gameObject.SetActive(true);
            if (view.HoldIcon.sprite != null) GameObject.Destroy(view.HoldIcon.sprite);
            view.HoldIcon.sprite = Texture2DPool.CreateSprite((Texture2D)elementC.icon.texture);
            HoldEntity = elementC.entity;
        }
        private void ClearHold()
        {
            view.HoldIcon.gameObject.SetActive(false);
            HoldEntity = null;
        }
        void Update()
        {
            if (HoldEntity == null) return;
            if (Input.GetKeyDown(KeyCode.Mouse1)) { ClearHold(); return; }
            Vector3 m = Input.mousePosition;
            m.z = 1;
            m = Camera.main.ScreenToWorldPoint(m);
            Vector2Int blockPos = Block.WorldToBlock(m);
            Vector2Int global = Block.WorldToGlobal(m);
            Vector2Byte pixelPos = Block.WorldToPixel(m);
            view.HoldIcon.transform.position = Block.GlobalToWorld(global);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                BuildingBase building = null;
                if (view.Dropdown.value != 0)
                {
                    building = BuildingBase.New(DerivedBuildingBaseList[view.Dropdown.value - 1], Guid.NewGuid().ToString());
                    building.TriggerCollider.size = new Vector2(HoldEntity.width, HoldEntity.height) * Pixel.Size;
                    building.TriggerCollider.offset = building.TriggerCollider.size / 2f;
                    building.TriggerCollider.transform.position = Block.GlobalToWorld(global);
                    building.global = global;
                    building.Size = new Vector2Int(HoldEntity.width, HoldEntity.height);
                    building.PorB = !view.Toggle.isOn;

                    SceneManager.Inst.NowScene.BuildingInRAM.Add(building.GUID, building);
                    SceneManager.Inst.NowScene.sceneCatalog.buildingTypeDic.Add(building.GUID, building.GetType());
                    building.scene = SceneManager.Inst.NowScene;
                }

                for (int y = 0; y < HoldEntity.height; y++)
                    for (int x = 0; x < HoldEntity.width; x++)
                    {
                        string typeName = HoldEntity.pixels[y * HoldEntity.width + x].typeName;
                        string colorName = HoldEntity.pixels[y * HoldEntity.width + x].colorName;
                        if (typeName == "¿ÕÆø") continue;
                        Vector2Int nowGloab = global + new Vector2Int(x, y);
                        BlockBase block = null;
                        if (view.Toggle.isOn)
                            block = SceneManager.Inst.NowScene.GetBackground(Block.GlobalToBlock(nowGloab));
                        else
                            block = SceneManager.Inst.NowScene.GetBlock(Block.GlobalToBlock(nowGloab));

                        Pixel pixel = Pixel.TakeOut(typeName, colorName, Block.GlobalToPixel(nowGloab));
                        if (building != null)
                        {
                            Building_Pixel building_Pixel = Building_Pixel.TakeOut().Init(pixel, new(x, y));
                            building.Deserialize_AddBuilding_Pixel(building_Pixel);
                            building.Deserialize_PixelSwitch(building_Pixel, pixel);
                        }
                        block.SetPixel(pixel);
                        block.DrawPixelAsync();
                    }
                if (building != null)
                    building.Init();
            }
        }
    }
}