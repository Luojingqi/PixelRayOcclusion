using PRO.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_Ray_Disk : AttackTestSlice2DBase_Disk
    {
        public Vector2 direction = Vector2.right;
        public float distance = 1;

        public AllowLogicChangeValue_AttackTestSlice2D_Ray_Disk changeValue = new();

        public override void UpdateFrame(SkillPlayAgent agent, SkillVisual_Disk visual, IEnumerable<SkillLogicBase> logics, FrameData frameData)
        {
            foreach (var logic in logics)
                logic.Before_AttackTest2D(this, frameData);

            Matrix4x4 trs = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                           Matrix4x4.TRS(changeValue.position, Quaternion.identity, Vector3.one);

            var array = TakeOut();
            int length = Physics2D.RaycastNonAlloc(
                trs.GetPosition(),
                trs.rotation * changeValue.direction,
                array, changeValue.distance, changeValue.layerMask);
#if UNITY_EDITOR
            Debug.DrawRay(
                trs.GetPosition(),
                trs.rotation * changeValue.direction * changeValue.distance,
                Color.green, visual.FrameTime / 1000f);
#endif
            foreach (var logic in logics)
                logic.Agoing_AttackTest2D(this, frameData, array.AsSpan(0, length));
            PutIn(array, length);

            if (frameData.sliceFrame == frameLength - 1)
                foreach (var logic in logics)
                    logic.After_AttackTest2D(this, frameData);

            changeValue.Reset(this);
        }

        public class AllowLogicChangeValue_AttackTestSlice2D_Ray_Disk : AllowLogicChangeValue_AttackTestSlice2DBase_Disk
        {
            public Vector2 direction;
            public float distance;
            public void Reset(AttackTestSlice2D_Ray_Disk slice)
            {
                base.Reset(slice);
                direction = slice.direction;
                distance = slice.distance;
            }
        }
    }
}
