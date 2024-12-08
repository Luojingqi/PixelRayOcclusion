using System.Collections.Generic;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class AttackTestSlice2D_Rect_Disk : AttackTestSlice2DBase_Disk
    {

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            RaycastHit2D[] data = Physics2D.BoxCastAll(position, scale, rotation.eulerAngles.z, Vector2.zero);
            if (agent.AttackTestTrack2DDic.TryGetValue(name, out List<RaycastHit2D> list) == false) { list = SkillPlayAgent.ListPool.TakeOut(); agent.AttackTestTrack2DDic.Add(name, list); }
            foreach (var item in data)
            {
                list.Add(item);
            }
        }
    }
}