using PRO.SkillEditor;
using System;
using UnityEngine;

namespace PRO
{
    public class Particle : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }
        public Rigidbody2D Rig2D { get; private set; }
        public Collider2D Collider { get; private set; }
        public SkillPlayAgent SkillPlayAgent { get; private set; }

        private Vector2Int surviveTimeRange = new Vector2Int(int.MaxValue, int.MaxValue);
        /// <summary>
        /// 存活的时间区间，设置时会随机指定remainTime（剩余存活时间）
        /// </summary>
        public Vector2Int SurviveTimeRange
        {
            get { return surviveTimeRange; }
            set
            {
                surviveTimeRange = value;
                RemainTime = UnityEngine.Random.Range(SurviveTimeRange.x, SurviveTimeRange.y);
            }
        }
        /// <summary>
        /// 粒子剩余的存活时间
        /// </summary>
        public int RemainTime
        {
            get { return remainTime; }
            set
            {
                remainTime = value;
                elapsedTime = 0;
            }
        }
        private int remainTime = int.MaxValue;


        private int elapsedTime;
        /// <summary>
        /// 粒子已经存活的时间
        /// </summary>
        public int ElapsedTime => elapsedTime;
        /// <summary>
        /// 粒子池的加载路径
        /// </summary>
        public string loadPath { get; private set; }

        public bool Active { get; set; }
        public bool RecyleState { get; private set; }
        public void UpdateRemainTime(int cutDown)
        {
            if (SceneManager.Inst.NowScene.GetBlock(Block.WorldToBlock(transform.position)) == null)
            {
                ParticleManager.Inst.GetPool(loadPath).PutIn(this.gameObject);
                return;
            }
            // transform.rotation = Quaternion.FromToRotation(Vector3.up, Rig2D.velocity);
            remainTime -= cutDown;
            elapsedTime += cutDown;
            UpdateEvent?.Invoke(this);
            if (RemainTime <= 0) RemainTimeIsZeroEvent?.Invoke(this);
        }

        public virtual void Init(string loadPath)
        {
            this.loadPath = loadPath;
            Renderer = GetComponent<SpriteRenderer>();
            Rig2D = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            SkillPlayAgent = GetComponent<SkillPlayAgent>();
            SkillPlayAgent?.Init();
        }

        public virtual void TakeOut()
        {
            ParticleManager.Inst.ActiveParticleHash.Add(this);
            RemainTime = UnityEngine.Random.Range(SurviveTimeRange.x, SurviveTimeRange.y);
            Active = true;
            RecyleState = false;
        }
        public virtual void PutIn()
        {
            transform.rotation = Quaternion.identity;
            if (Rig2D != null)
            {
                Rig2D.velocity = Vector2.zero;
                Rig2D.angularVelocity = 0;
                Rig2D.simulated = true;
            }
            Renderer.color = Color.white;
            Renderer.flipY = false;

            UpdateEvent = null;
            CollisionEnterEvent = null;
            CollisionExitEvent = null;
            RemainTimeIsZeroEvent = null;
            RemainTime = int.MaxValue;

            if (Collider != null) Collider.enabled = true;
            gameObject.layer = 10;

            if (SkillPlayAgent != null)
            {
                SkillPlayAgent.idle = null;
                SkillPlayAgent.Skill = null;
                SkillPlayAgent.ClearTime();
            }
            Active = false;
            RecyleState = true;
        }
        public event Action<Particle> UpdateEvent;
        /// <summary>
        /// 发生碰撞事件
        /// </summary>
        public event Action<Particle, Collision2D> CollisionEnterEvent;
        /// <summary>
        /// 碰撞退出事件
        /// </summary>
        public event Action<Particle, Collision2D> CollisionExitEvent;
        /// <summary>
        /// 存活事件结束事件
        /// </summary>
        public event Action<Particle> RemainTimeIsZeroEvent;
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEnterEvent?.Invoke(this, collision);
        }
        protected void OnCollisionExit2D(Collision2D collision)
        {
            CollisionExitEvent?.Invoke(this, collision);
        }

        private static float pixelSizeHalf = Pixel.Size / 2;
        public void SetGlobal(Vector2Int global)
        {
            transform.position = Block.GlobalToWorld(global) + new Vector3(pixelSizeHalf, pixelSizeHalf);
        }
    }
}
