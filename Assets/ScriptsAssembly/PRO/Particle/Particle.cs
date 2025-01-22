using PRO.SkillEditor;
using System;
using UnityEngine;

namespace PRO
{
    public class Particle : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public Collider2D Collider { get; private set; }
        public SkillPlayAgent SkillPlayAgent { get; private set; }

        private Vector2Int surviveTimeRange = new Vector2Int(int.MaxValue, int.MaxValue);
        public Vector2Int SurviveTimeRange
        {
            get { return surviveTimeRange; }
            set
            {
                surviveTimeRange = value;
                remainTime = UnityEngine.Random.Range(SurviveTimeRange.x, SurviveTimeRange.y);
            }
        }
        /// <summary>
        /// 粒子剩余的存活时间
        /// </summary>
        public int remainTime { get; private set; } = int.MaxValue;

        public string loadPath { get; private set; }

        public void UpdateRemainTime(int cutDown)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, Rigidbody.velocity);
            remainTime -= cutDown;
            if (remainTime <= 0) RemainTimeIsZeroEvent?.Invoke(this);
        }

        public void Init(string loadPath)
        {
            this.loadPath = loadPath;
            Renderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            SkillPlayAgent = GetComponent<SkillPlayAgent>();
        }

        public void TakeOut()
        {
            ParticleManager.Inst.ActiveParticleHash.Add(this);
            remainTime = UnityEngine.Random.Range(SurviveTimeRange.x, SurviveTimeRange.y);
        }
        public void PutIn()
        {
            transform.rotation = Quaternion.identity;
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.angularVelocity = 0;
            Renderer.color = Color.white;
            CollisionEnterEvent = null;
            CollisionExitEvent = null;
            RemainTimeIsZeroEvent = null;
            remainTime = int.MaxValue;
            Rigidbody.simulated = false;
            Collider.enabled = true;
            gameObject.layer = 10;
            ParticleManager.Inst.ActiveParticleHash.Remove(this);
        }

        public event Action<Particle, Collision2D> CollisionEnterEvent;
        public event Action<Particle, Collision2D> CollisionExitEvent;
        public event Action<Particle> RemainTimeIsZeroEvent;
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEnterEvent?.Invoke(this, collision);
        }
        protected void OnCollisionExit2D(Collision2D collision)
        {
            CollisionExitEvent?.Invoke(this, collision);
        }
    }
}
