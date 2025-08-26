using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PRO.Weapon.WeaponBase;

namespace PRO.SkillEditor
{
    internal abstract class AttackTestSlice2DBase : SliceBase
    {
        public override void DrawHandle(SkillPlayAgent agent)
        {
            var rts = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, Vector3.one);
            Vector3 position = DiskData_AT.position;
            if (HandlePosition(rts, ref position))
                DiskData_AT.position = position;
        }

        public AttackTestSlice2DBase_Disk DiskData_AT { get => DiskData as AttackTestSlice2DBase_Disk; }

        [LabelText("位置")]
        [ShowInInspector]
        public Vector2 Position { get => DiskData_AT.position; set => DiskData_AT.position = value; }

        [LabelText("检测层")]
        [ShowInInspector, EnumToggleButtons]
        public GameLayer LayerMask { get => (GameLayer)DiskData_AT.layerMask; set => DiskData_AT.layerMask = (int)value; }

        private List<GameLayer> layerMaskList;
        [LabelText("层级")]
        [ShowInInspector]
        public List<GameLayer> LayerMaskList
        {
            get
            {
                if (layerMaskList != null) return layerMaskList;
                layerMaskList = new List<GameLayer>();
                int layerMask = DiskData_AT.layerMask;
                var allLayers = Enum.GetValues(typeof(GameLayer)).Cast<GameLayer>();

                foreach (var layer in allLayers)
                {
                    int layerValue = (int)layer;
                    if (layerValue < 0 || layerValue >= 32) continue; // 确保是有效的层级
                    int mask = 1 << layerValue;
                    if ((layerMask & mask) != 0)
                        layerMaskList.Add(layer);
                }
                return layerMaskList;
            }
            set
            {
                DiskData_AT.layerMask = 0;
                layerMaskList = value;
                foreach (var layer in layerMaskList)
                    DiskData_AT.layerMask |= 1 << (int)layer;
            }
        }
    }
}
