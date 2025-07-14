using PRO.SkillEditor;
using System;
using UnityEngine;
using PRO.Proto.Ex;
using Google.FlatBuffers;
using PRO.Flat.Ex;
using Sirenix.OdinInspector;

namespace PRO
{
    public class Particle : MonoBehaviour, IScene
    {
        public SpriteRenderer Renderer { get; private set; }
        public Rigidbody2D Rig2D { get; private set; }
        public Collider2D Collider { get; private set; }
        public SkillPlayAgent SkillPlayAgent { get; private set; }

        private Vector2Int surviveTimeRange = new Vector2Int(int.MaxValue, int.MaxValue);
        /// <summary>
        /// 存活的时间区间，设置时会随机指定remainTime（剩余存活时间）ms
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
        /// 粒子剩余的存活时间 ms
        /// 为0会被回收
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
        /// 粒子已经存活的时间 ms
        /// </summary>
        public int ElapsedTime => elapsedTime;
        /// <summary>
        /// 粒子池的加载路径
        /// </summary>
        public string loadPath { get; private set; }
        /// <summary>
        /// 当前是否已被回收
        /// </summary>
        public bool RecyleState { get; private set; }

        public SceneEntity Scene => _scene;
        private SceneEntity _scene;

        public void UpdateRemainTime(int cutDown)
        {
            // transform.rotation = Quaternion.FromToRotation(Vector3.up, Rig2D.velocity);
            remainTime -= cutDown;
            elapsedTime += cutDown;
            UpdateEvent?.Invoke();
            if (RemainTime <= 0) RemainTimeIsZeroEvent?.Invoke();
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

        public virtual void TakeOut(SceneEntity scene)
        {
            RemainTime = UnityEngine.Random.Range(SurviveTimeRange.x, SurviveTimeRange.y);
            RecyleState = false;
            _scene = scene;
        }
        /// <summary>
        /// 不可调用，请获取对应池然后使用池的PutIn
        /// </summary>
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
            RemainTime = int.MinValue;

            if (Collider != null) Collider.enabled = true;
            gameObject.layer = (int)GameLayer.Particle;

            if (SkillPlayAgent != null)
            {
                SkillPlayAgent.idle = null;
                SkillPlayAgent.Skill = null;
                SkillPlayAgent.ClearTimeAndBuffer();
            }
            RecyleState = true;
        }
        public event Action UpdateEvent;
        /// <summary>
        /// 发生碰撞事件
        /// </summary>
        public event Action<Collision2D> CollisionEnterEvent;
        /// <summary>
        /// 碰撞退出事件
        /// </summary>
        public event Action<Collision2D> CollisionExitEvent;
        /// <summary>
        /// 存活事件结束事件
        /// </summary>
        public event Action RemainTimeIsZeroEvent;
        protected void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEnterEvent?.Invoke(collision);
        }
        protected void OnCollisionExit2D(Collision2D collision)
        {
            CollisionExitEvent?.Invoke(collision);
        }


        private static float pixelSizeHalf = Pixel.Size / 2;
        public void SetGlobal(Vector2Int global)
        {
            transform.position = Block.GlobalToWorld(global) + new Vector3(pixelSizeHalf, pixelSizeHalf);
        }

        public Offset<Flat.ParticleData> ToDisk(FlatBufferBuilder builder)
        {
            var loadPathOffset = builder.CreateString(loadPath);
            var extendDataOffset = ExtendDataToDisk(builder);

            Flat.ParticleData.StartParticleData(builder);
            Flat.ParticleData.AddLoadPath(builder, loadPathOffset);
            Flat.ParticleData.AddTransform(builder, transform.ToDisk(builder));
            Flat.ParticleData.AddRigidbody(builder, Rig2D.ToDisk(builder));
            Flat.ParticleData.AddSurviveTimeRange(builder, surviveTimeRange.ToDisk(builder));
            Flat.ParticleData.AddRemainTime(builder, remainTime);
            Flat.ParticleData.AddElapsedTime(builder, elapsedTime);
            Flat.ParticleData.AddExtendData(builder, extendDataOffset);
            return Flat.ParticleData.EndParticleData(builder);
        }
        public virtual int ExtendDataToDisk(FlatBufferBuilder builder)
        {
            return 0;
        }

        public virtual void ToRAM(Flat.ParticleData diskData)
        {
            transform.ToRAM(diskData.Transform.Value);
            Rig2D.ToRAM(diskData.Rigidbody.Value);
            surviveTimeRange = diskData.SurviveTimeRange.Value.ToRAM();
            remainTime = diskData.RemainTime;
            elapsedTime = diskData.ElapsedTime;
        }
    }
}
