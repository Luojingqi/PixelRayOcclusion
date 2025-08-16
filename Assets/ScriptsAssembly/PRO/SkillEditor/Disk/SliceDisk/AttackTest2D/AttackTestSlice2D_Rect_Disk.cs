using PRO.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_Rect_Disk : AttackTestSlice2DBase_Disk
    {
        public float angle;
        public Vector2 scale = Vector2.one;

        public new AllowLogicChangeValue_AttackTestSlice2D_Rect_Disk changeValue = new();
        public AttackTestSlice2D_Rect_Disk() => base.changeValue = changeValue;


        public override void UpdateFrame(SkillPlayAgent agent, SkillPlayData playData, FrameData frameData)
        {
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Before_AttackTest2D(this, frameData);
            var array = TakeOut();
            var trs = Matrix4x4.TRS(agent.transform.position, Quaternion.Euler(0, 0, agent.transform.rotation.eulerAngles.z), Vector3.one) *
                      Matrix4x4.TRS(changeValue.position, Quaternion.Euler(0, 0, changeValue.angle), changeValue.scale);
            int length = Physics2D.BoxCastNonAlloc(
                trs.GetPosition(),
                trs.lossyScale,
                trs.rotation.eulerAngles.z,
                Vector2.zero,
                array, 0, changeValue.layerMask);
#if UNITY_EDITOR
            var scale_2 = Vector2.one / 2f;
            var time = playData.SkillVisual.FrameTime / 1000f;
            var pos0 = trs.MultiplyPoint(new Vector2(scale_2.x, scale_2.y));
            var pos1 = trs.MultiplyPoint(new Vector2(-scale_2.x, scale_2.y));
            var pos2 = trs.MultiplyPoint(new Vector2(-scale_2.x, -scale_2.y));
            var pos3 = trs.MultiplyPoint(new Vector2(scale_2.x, -scale_2.y));
            Debug.DrawLine(pos0, pos1, Color.green, time);
            Debug.DrawLine(pos1, pos2, Color.green, time);
            Debug.DrawLine(pos2, pos3, Color.green, time);
            Debug.DrawLine(pos3, pos0, Color.green, time);
#endif
            for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                playData.SkillLogicList[logicIndex].Agoing_AttackTest2D(this, frameData, array.AsSpan(0, length));
            PutIn(array, length);

            if (frameData.sliceFrame == frameLength - 1)
                for (int logicIndex = 0; logicIndex < playData.SkillLogicList.Count; logicIndex++)
                    playData.SkillLogicList[logicIndex].After_AttackTest2D(this, frameData);

            changeValue.Reset(this);
        }

        public class AllowLogicChangeValue_AttackTestSlice2D_Rect_Disk : AllowLogicChangeValue_AttackTestSlice2DBase_Disk
        {
            public float angle;
            public Vector2 scale;

            public void Reset(AttackTestSlice2D_Rect_Disk slice)
            {
                base.Reset(slice);
                angle = slice.angle;
                scale = slice.scale;
            }
        }
    }
}