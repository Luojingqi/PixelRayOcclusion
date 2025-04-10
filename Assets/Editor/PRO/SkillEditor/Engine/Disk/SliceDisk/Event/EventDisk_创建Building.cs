using PROTool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class EventDisk_创建Building : EventSlice_DiskBase
    {

        public Type type;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            var building = BuildingBase.New(type, Guid.NewGuid().ToString());
            building.TriggerCollider.size = new Vector2(HoldEntity.width, HoldEntity.height) * Pixel.Size;
            building.TriggerCollider.offset = building.TriggerCollider.size / 2f;
            building.TriggerCollider.transform.position = Block.GlobalToWorld(global);
            building.global = global;
            building.Size = new Vector2Int(HoldEntity.width, HoldEntity.height);
            building.PorB = !view.Toggle.isOn;

            SceneManager.Inst.NowScene.BuildingInRAM.Add(building.GUID, building);
            SceneManager.Inst.NowScene.sceneCatalog.buildingTypeDic.Add(building.GUID, building.GetType());
        }
    }
}
