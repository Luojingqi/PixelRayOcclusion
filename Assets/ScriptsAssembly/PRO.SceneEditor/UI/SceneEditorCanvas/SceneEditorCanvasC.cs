using PRO.Tool;
using PROTool;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PRO.SceneEditor
{
    public class SceneEditorCanvasC : UIControllerBase, ITime_Start, ITime_Update
    {
        public override UIViewBase View => view;
        private SceneEditorCanvasV view = new SceneEditorCanvasV();

        public static SceneEditorCanvasC Inst;

        private List<Type> DerivedBuildingBaseList = ReflectionTool.GetDerivedClasses(typeof(BuildingBase));
        public override void Init(string uiName)
        {
            base.Init(uiName);

            Inst = this;

            AddChildUI(view.ElementViewPanel);
            AddChildUI(view.FileDirectoryInfoTree);

            for (int i = 0; i < DerivedBuildingBaseList.Count; i++)
                view.Dropdown.options.Add(new TMP_Dropdown.OptionData(DerivedBuildingBaseList[i].Name));
        }

        public void TimeStart()
        {
            Init("123");
            //gameObject.SetActive(false);
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
        public override void TimeUpdate()
        {
            base.TimeUpdate();
            if (HoldEntity == null) return;
            if (Input.GetKeyDown(KeyCode.Mouse1)) { ClearHold(); return; }
            Vector2Int global = MousePoint.globalPos;
            view.HoldIcon.transform.position = Block.GlobalToWorld(MousePoint.globalPos);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                BuildingBase building = null;
                if (view.Dropdown.value != 0)
                {
                    building = BuildingBase.New(DerivedBuildingBaseList[view.Dropdown.value - 1], Guid.NewGuid().ToString(), SceneManager.Inst.NowScene);
                    building.TriggerCollider.size = new Vector2(HoldEntity.width, HoldEntity.height) * Pixel.Size;
                    building.TriggerCollider.offset = building.TriggerCollider.size / 2f;
                    building.transform.position = Block.GlobalToWorld(global);
                    building.Global = global;
                    building.Size = new Vector2Int(HoldEntity.width, HoldEntity.height);

                    building.Scene.BuildingInRAM.Add(building.GUID, building);
                }

                for (int y = 0; y < HoldEntity.height; y++)
                    for (int x = 0; x < HoldEntity.width; x++)
                    {
                        string typeName = HoldEntity.pixels[y * HoldEntity.width + x].typeName;
                        string colorName = HoldEntity.pixels[y * HoldEntity.width + x].colorName;
                        if (typeName == "¿ÕÆø") continue;
                        Vector2Int nowGloab = global + new Vector2Int(x, y);
                        BlockBase blockBase = SceneManager.Inst.NowScene.GetBlockBase(view.Toggle.isOn ? BlockBase.BlockType.BackgroundBlock : BlockBase.BlockType.Block, Block.GlobalToBlock(nowGloab));
                        if (blockBase == null) continue;
                        Pixel pixel = Pixel.TakeOut(typeName, colorName, Block.GlobalToPixel(nowGloab));
                        blockBase.SetPixel(pixel);
                        if (building != null)
                        {
                            Building_Pixel building_Pixel = Building_Pixel.TakeOut().Init(pixel, new(x, y));
                            building.ToRAM_AddBuilding_Pixel(building_Pixel);
                            building.ToRAM_PixelSwitch(building_Pixel, pixel);
                        }
                    }
                if (building != null)
                    building.Init();
            }
        }
    }
}