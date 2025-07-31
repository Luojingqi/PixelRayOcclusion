using UnityEngine;

namespace PRO.SkillEditor
{
    public class EventDisk_旋风 : EventDisk_Base
    {
        public Vector2 position;
        public int radius;
        public float force;

        public override void UpdateFrame(SkillPlayAgent agent, int frame, int frameIndex, int trackIndex)
        {
            Vector2 center = agent.transform.position + agent.transform.rotation * position;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(center, radius * Pixel.Size, Vector2.zero, 0,
                1 << (int)GameLayer.Role | 1 << (int)GameLayer.UnRole |
                1 << (int)GameLayer.Particle | 1 << (int)GameLayer.Particle_Role |
                1 << (int)GameLayer.Particle_Block | 1 << (int)GameLayer.Particle_Block_Role);
            foreach (var hit in hits)
            {
                hit.rigidbody.AddForce((center - (Vector2)hit.transform.position).normalized * force, ForceMode2D.Force);
            }
        }
    }
}
