using System;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_Capsule_Disk : AttackTestSlice2DBase_Disk
    {
        public float angle;
        public Vector2 scale = new Vector2(1, 1);

        public new AllowLogicChangeValue_AttackTestSlice2D_Capsule_Disk changeValue = new();
        public AttackTestSlice2D_Capsule_Disk() => base.changeValue = changeValue;

        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Before_AttackTest2D(this, frameData);

            var array = TakeOut();
            var trs = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                      Matrix4x4.TRS(changeValue.position, Quaternion.Euler(0, 0, changeValue.angle), changeValue.scale);
            int length = Physics2D.CapsuleCastNonAlloc(
                trs.GetPosition(),
                trs.lossyScale,
                CapsuleDirection2D.Vertical,
                trs.rotation.eulerAngles.z,
                Vector2.zero,
                array, 0, changeValue.layerMask);
#if UNITY_EDITOR
            var time = playData.SkillVisual.FrameTime / 1000f;
            DrawCapsule(trs.GetPosition(), trs.lossyScale, trs.rotation.eulerAngles.z, CapsuleDirection2D.Vertical, Color.green, time);
#endif
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Agoing_AttackTest2D(this, frameData, array.AsSpan(0, length));
            PutIn(array, length);

            if (frameData.sliceFrame == frameLength - 1)
                for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                    playData.SkillLogicList[logicIndex].After_AttackTest2D(this, frameData);

            changeValue.Reset(this);
        }
#if UNITY_EDITOR
        public static void DrawCapsule(Vector2 position, Vector2 scale, float angle, CapsuleDirection2D direction, Color color, float duration = 0)
        {
            // 计算旋转矩阵
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            // 根据方向确定半径和高度
            float radius, height;
            if (direction == CapsuleDirection2D.Vertical)
            {
                radius = scale.x * 0.5f;
                height = Mathf.Max(scale.y - scale.x, 0);
            }
            else
            {
                radius = scale.y * 0.5f;
                height = Mathf.Max(scale.x - scale.y, 0);
            }

            // 计算半圆中心位置
            Vector2 topCenter, bottomCenter;
            if (direction == CapsuleDirection2D.Vertical)
            {
                topCenter = position + new Vector2(0, height * 0.5f);
                bottomCenter = position - new Vector2(0, height * 0.5f);
            }
            else
            {
                topCenter = position + new Vector2(height * 0.5f, 0);
                bottomCenter = position - new Vector2(height * 0.5f, 0);
            }

            // 旋转半圆中心
            topCenter = RotatePoint(topCenter, position, cos, sin);
            bottomCenter = RotatePoint(bottomCenter, position, cos, sin);

            // 绘制两个半圆
            DrawArc(topCenter, radius, angle, direction == CapsuleDirection2D.Vertical ? 0 : 90, 180, color, duration);
            DrawArc(bottomCenter, radius, angle, direction == CapsuleDirection2D.Vertical ? 180 : 270, 180, color, duration);

            // 绘制两侧直线
            Vector2 sideDir = direction == CapsuleDirection2D.Vertical ?
                new Vector2(cos, sin) : new Vector2(-sin, cos);
            Vector2 sideOffset = sideDir * radius;

            Vector2 topLineStart = topCenter - sideOffset;
            Vector2 topLineEnd = bottomCenter - sideOffset;
            Vector2 bottomLineStart = topCenter + sideOffset;
            Vector2 bottomLineEnd = bottomCenter + sideOffset;

            Debug.DrawLine(topLineStart, topLineEnd, color, duration);
            Debug.DrawLine(bottomLineStart, bottomLineEnd, color, duration);
        }

        private static Vector2 RotatePoint(Vector2 point, Vector2 center, float cos, float sin)
        {
            Vector2 translated = point - center;
            return new Vector2(
                translated.x * cos - translated.y * sin + center.x,
                translated.x * sin + translated.y * cos + center.y
            );
        }

        private static void DrawArc(Vector2 center, float radius, float rotation, float startAngle, float arcAngle, Color color, float duration)
        {
            int segments = 20;
            float step = arcAngle / segments * Mathf.Deg2Rad;
            float startRad = (startAngle + rotation) * Mathf.Deg2Rad;

            Vector2 prevPoint = center + new Vector2(
                Mathf.Cos(startRad) * radius,
                Mathf.Sin(startRad) * radius
            );

            for (int i = 1; i <= segments; i++)
            {
                float angle = startRad + step * i;
                Vector2 nextPoint = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                );

                Debug.DrawLine(prevPoint, nextPoint, color, duration);
                prevPoint = nextPoint;
            }
        }
#endif

        public class AllowLogicChangeValue_AttackTestSlice2D_Capsule_Disk : AllowLogicChangeValue_AttackTestSlice2DBase_Disk
        {
            public float angle;
            public Vector2 scale;

            public void Reset(AttackTestSlice2D_Capsule_Disk slice)
            {
                base.Reset(slice);
                angle = slice.angle;
                scale = slice.scale;
            }
        }
    }
}