using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_Rect_Disk : AttackTestSlice2DBase_Disk
    {
        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            Quaternion quaternion = rotation * agent.transform.rotation;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(agent.transform.position + agent.transform.rotation * position, scale, quaternion.eulerAngles.z, Vector2.zero, 0, 1 << (int)GameLayer.Role);

            var data = agent.GetBufferData<BufferData>(this);
            if (data == null)
            {
                data = BufferData.TakeOut();
                agent.AddBufferData(this, data);
            }

            foreach (var hit in hits)
            {
                data.value.Add(hit);
            }
        }
    }
}