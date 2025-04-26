using System;
using UnityEngine;

namespace PRO.SkillEditor
{
    public class EventDisk_吹风 : EventDisk_Base
    {
        public Vector2 position;
        public Quaternion rotation = Quaternion.identity;
        public Vector2 scale = Vector2.one;

        public float force;
        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            Quaternion quaternion = rotation * agent.transform.rotation;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(agent.transform.position + agent.transform.rotation * position, scale, quaternion.eulerAngles.z, Vector2.zero, 0,
                1 << (int)GameLayer.Role | 1 << (int)GameLayer.UnRole |
                1 << (int)GameLayer.Particle | 1 << (int)GameLayer.Particle_Role |
                1 << (int)GameLayer.Particle_Block | 1 << (int)GameLayer.Particle_Block_Role);
            foreach (var hit in hits)
            {
                hit.rigidbody.AddForce(quaternion * Vector2.right * force, ForceMode2D.Force);
                Debug.Log(hit.transform.name + "|" + quaternion * Vector2.right);
            }
        }
    }
}
