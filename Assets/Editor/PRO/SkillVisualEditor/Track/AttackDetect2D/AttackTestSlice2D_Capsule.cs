using Sirenix.OdinInspector;
using UnityEngine;

namespace PRO.SkillEditor
{
    internal class AttackTestSlice2D_Capsule : AttackTestSlice2DBase
    {
        public override void Init(Slice_DiskBase sliceDisk)
        {
            base.Init(sliceDisk);
            if (sliceDisk.startFrame == -1)
            {
                Name = "胶囊";
            }
        }

        private AttackTestSlice2D_Capsule_Disk diskData => DiskData as AttackTestSlice2D_Capsule_Disk;

        public override void DrawGizmo(SkillPlayAgent agent)
        {
            DrawCapsuleGizmo(Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one),
                diskData.position, diskData.scale, diskData.angle, CapsuleDirection2D.Vertical);
        }
        #region Gizmo绘制
        public static void DrawCapsuleGizmo(Matrix4x4 trs, Vector2 position, Vector2 scale, float angle, CapsuleDirection2D direction)
        {

            // 设置新的 Gizmos 矩阵（应用位置和旋转）
            Matrix4x4 rotationMatrix = trs * Matrix4x4.TRS(
                                        position,
                                        Quaternion.Euler(0, 0, angle),
                                        Vector3.one);
            Gizmos.matrix = rotationMatrix;

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
                topCenter = new Vector2(0, height * 0.5f);
                bottomCenter = new Vector2(0, -height * 0.5f);
            }
            else
            {
                topCenter = new Vector2(height * 0.5f, 0);
                bottomCenter = new Vector2(-height * 0.5f, 0);
            }

            // 绘制两个半圆
            DrawHalfCircle(topCenter, radius, direction == CapsuleDirection2D.Vertical ? 0 : 90);
            DrawHalfCircle(bottomCenter, radius, direction == CapsuleDirection2D.Vertical ? 180 : 270);

            // 绘制两侧直线
            Vector2 sideDir = direction == CapsuleDirection2D.Vertical ?
                Vector2.right : Vector2.up;
            Vector2 sideOffset = sideDir * radius;

            Vector2 topLineStart = topCenter - sideOffset;
            Vector2 topLineEnd = bottomCenter - sideOffset;
            Vector2 bottomLineStart = topCenter + sideOffset;
            Vector2 bottomLineEnd = bottomCenter + sideOffset;

            Gizmos.DrawLine(topLineStart, topLineEnd);
            Gizmos.DrawLine(bottomLineStart, bottomLineEnd);
        }

        private static void DrawHalfCircle(Vector2 center, float radius, float startAngle)
        {
            int segments = 20;
            float arcAngle = 180f;
            float step = arcAngle / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = startAngle + step * i;
                float angle2 = startAngle + step * (i + 1);

                Vector3 point1 = center + new Vector2(
                    Mathf.Cos(angle1 * Mathf.Deg2Rad) * radius,
                    Mathf.Sin(angle1 * Mathf.Deg2Rad) * radius
                );

                Vector3 point2 = center + new Vector2(
                    Mathf.Cos(angle2 * Mathf.Deg2Rad) * radius,
                    Mathf.Sin(angle2 * Mathf.Deg2Rad) * radius
                );

                Gizmos.DrawLine(point1, point2);
            }
        }
        #endregion
        public override void DrawHandle(SkillPlayAgent agent)
        {
            base.DrawHandle(agent);
            Quaternion rotation = Quaternion.Euler(0, 0, diskData.angle);
            if (HandleRotation(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, Quaternion.identity, Vector3.one),
                ref rotation))
                Angle = rotation.eulerAngles.z;

            Vector3 scale = diskData.scale;
            if (HandleScale(
                Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                Matrix4x4.TRS(diskData.position, rotation, Vector3.one),
                ref scale))
                Scale = scale;
        }

        [LabelText("旋转")]
        [ShowInInspector]
        public float Angle { get => diskData.angle; set => diskData.angle = value % 360; }
        [LabelText("大小")]
        [ShowInInspector]
        public Vector2 Scale { get => diskData.scale; set => diskData.scale = new Vector2(value.x * Mathf.Sign(value.x), value.y * Mathf.Sign(value.y)); }

    }
}
