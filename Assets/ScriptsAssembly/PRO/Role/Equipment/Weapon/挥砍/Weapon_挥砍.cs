using PRO.SkillEditor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.Weapon
{
    public class Weapon_挥砍 : WeaponBase
    {
        [ShowInInspector] public SkillPlayData 前摇 { get => PlayDataArray[0]; set => PlayDataArray[0] = value; }
        [ShowInInspector] public SkillPlayData 检测 { get => PlayDataArray[1]; set => PlayDataArray[1] = value; }
        [ShowInInspector] public SkillPlayData 后摇 { get => PlayDataArray[2]; set => PlayDataArray[2] = value; }

        public int Auto_挥砍距离;

        public int Auto_最多目标数;

        public Weapon_挥砍(EquipmentPrefix prefix) : base(prefix)
        {
            前摇.SkillLogicList.Add(new Weapon_挥砍_前摇_Logic_应用攻速(this));
            检测.SkillLogicList.Add(new Weapon_挥砍_检测_Logic_应用挥砍距离(this));
        }

        public class Weapon_挥砍_前摇_Logic_应用攻速 : SkillLogicBase
        {
            public Weapon_挥砍 Weapon;
            public Weapon_挥砍_前摇_Logic_应用攻速(Weapon_挥砍 weapon) { Weapon = weapon; }
            public override void Before_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual)
            {
                Weapon.前摇.scale = (float)Weapon.Agent.Info.攻击速度.ValueSum;
                Weapon.后摇.scale = (float)Weapon.Agent.Info.攻击速度.ValueSum;
                Weapon.后摇.scale = (float)Weapon.Agent.Info.攻击速度.ValueSum;
            }
        }
        public class Weapon_挥砍_检测_Logic_应用挥砍距离 : SkillLogicBase
        {
            public Weapon_挥砍 Weapon;
            public Weapon_挥砍_检测_Logic_应用挥砍距离(Weapon_挥砍 weapon) { Weapon = weapon; }
            public override void Before_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData)
            {
                if (slice is AttackTestSlice2D_FanShaped_Disk attackSlice)
                {
                    attackSlice.changeValue.distance = Pixel.Size * Weapon.Auto_挥砍距离;
                }
            }
        }
        public abstract class Weapon_挥砍_检测_Logic_最多目标数造成伤害 : SkillLogicBase
        {
            public Weapon_挥砍 Weapon;
            public Weapon_挥砍_检测_Logic_最多目标数造成伤害(Weapon_挥砍 weapon) { Weapon = weapon; roleSet = new(weapon.Auto_最多目标数); }
            private HashSet<Role> roleSet;
            public struct Hit
            {
                public RaycastHit2D raycastHit;
                public Role role;

                public Hit(RaycastHit2D raycastHit, Role role)
                {
                    this.raycastHit = raycastHit;
                    this.role = role;
                }
            }
            public override void Agoing_AttackTest2D(SkillPlayAgent agent, SkillPlayData playData, AttackTestSlice2DBase_Disk slice, FrameData frameData, Span<RaycastHit2D> hitSpan)
            {
                if (roleSet.Count >= Weapon.Auto_最多目标数) return;
                for (int i = 0; i < hitSpan.Length; i++)
                {
                    var hit = hitSpan[i];
                    if (hit.transform == Weapon.Agent.transform) continue;
                    var role = Weapon.Agent.Scene.GetRole(hit.transform);
                    if (roleSet.Contains(role) == false)
                    {
                        roleSet.Add(role);
                        造成伤害(new Hit(hit, role));
                    }
                }
            }

            protected abstract void 造成伤害(Hit hit);
        }
    }
}
