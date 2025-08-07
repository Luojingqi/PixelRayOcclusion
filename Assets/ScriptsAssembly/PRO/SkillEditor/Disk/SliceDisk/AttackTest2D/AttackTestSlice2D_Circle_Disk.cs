using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_Circle_Disk : AttackTestSlice2DBase_Disk
    {
        public float radius = 0.5f;

        public AllowLogicChangeValue_AttackTestSlice2D_Circle_Disk changeValue = new();

        public override void UpdateFrame(SkillPlayAgent agent, SkillVisual_Disk visual, IEnumerable<SkillLogicBase> logics, FrameData frameData)
        {
            foreach (var logic in logics)
                logic.Before_AttackTest2D(this, frameData);

            var array = TakeOut();
            var trs = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                      Matrix4x4.TRS(changeValue.position, Quaternion.identity, Vector3.one);
            int length = Physics2D.CircleCastNonAlloc(
                trs.GetPosition(),
                changeValue.radius,
                Vector2.zero,
                array, 0, changeValue.layerMask);
#if UNITY_EDITOR
            var time = visual.FrameTime / 1000f;
            DrawCircle(trs.GetPosition(), changeValue.radius, Color.green, time);
#endif
            foreach (var logic in logics)
                logic.Agoing_AttackTest2D(this, frameData, array.AsSpan(0, length));
            PutIn(array, length);

            if (frameData.sliceFrame == frameLength - 1)
                foreach (var logic in logics)
                    logic.After_AttackTest2D(this, frameData);

            changeValue.Reset(this);
        }
#if UNITY_EDITOR
        private void DrawCircle(Vector3 center, float radius, Color color, float duration, int segments = 32)
        {
            float angle = 0f;
            Vector3 lastPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

            for (int i = 1; i <= segments; i++)
            {
                angle = (float)i / segments * Mathf.PI * 2f;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                Debug.DrawLine(lastPoint, nextPoint, color, duration);
                lastPoint = nextPoint;
            }
        }
#endif
        public class AllowLogicChangeValue_AttackTestSlice2D_Circle_Disk : AllowLogicChangeValue_AttackTestSlice2DBase_Disk
        {
            public float radius;

            public void Reset(AttackTestSlice2D_Circle_Disk slice)
            {
                base.Reset(slice);
                radius = slice.radius;
            }
        }
    }
}