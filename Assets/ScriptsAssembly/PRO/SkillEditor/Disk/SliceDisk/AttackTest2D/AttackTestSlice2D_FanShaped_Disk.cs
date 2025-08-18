using System;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_FanShaped_Disk : AttackTestSlice2DBase_Disk
    {
        public Vector2 direction = Vector2.right;
        public float distance = 1;
        public float angle = 30f;
        public float density = 0.3f;

        public new AllowLogicChangeValue_AttackTestSlice2D_FanShaped_Disk changeValue = new();
        public AttackTestSlice2D_FanShaped_Disk() => base.changeValue = changeValue;

        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Before_AttackTest2D(agent, playData, this, frameData);

            int count = (int)(changeValue.density * changeValue.angle);
            Span<Vector2> span = stackalloc Vector2[count];
            CreateRayEndPos(span);
            var trs = Matrix4x4.TRS(agent.transform.position, agent.transform.rotation, Vector3.one) *
                           Matrix4x4.TRS(changeValue.position, Quaternion.identity, Vector3.one);
            var startPos = trs.GetPosition();
            for (int i = 0; i < span.Length; i++)
            {
                var array = TakeOut();
                int length = Physics2D.RaycastNonAlloc(
                    startPos,
                    trs.MultiplyPoint(span[i]),
                    array, 0, changeValue.layerMask);
                for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                    playData.SkillLogicList[logicIndex].Agoing_AttackTest2D(agent, playData, this, frameData, array.AsSpan(0, length));
                PutIn(array, length);
            }
#if UNITY_EDITOR
            var time = playData.SkillVisual.FrameTime / 1000f;
            for (int i = 0; i < span.Length; i++)
                Debug.DrawLine(startPos, trs.MultiplyPoint(span[i]), Color.green, time);
#endif
            if (frameData.sliceFrame == frameLength - 1)
                for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                    playData.SkillLogicList[logicIndex].After_AttackTest2D(agent, playData, this, frameData);

            changeValue.Reset(this);
        }

        public void CreateRayEndPos(Span<Vector2> span)
        {
            float halfAngle = changeValue.angle * 0.5f;
            float angleStep = changeValue.angle / Mathf.Max(1, span.Length - 1);
            for (int i = 0; i < span.Length; i++)
            {
                float currentAngle = -halfAngle + i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
                Vector2 endPoint = rotation * changeValue.direction * changeValue.distance;
                span[i] = endPoint;
            }
        }

        public class AllowLogicChangeValue_AttackTestSlice2D_FanShaped_Disk : AllowLogicChangeValue_AttackTestSlice2DBase_Disk
        {
            public Vector2 direction;
            public float distance;
            public float angle;
            public float density;

            public void Reset(AttackTestSlice2D_FanShaped_Disk slice)
            {
                base.Reset(slice);
                direction = slice.direction;
                distance = slice.distance;
                angle = slice.angle;
                density = slice.density;
            }
        }
    }
}