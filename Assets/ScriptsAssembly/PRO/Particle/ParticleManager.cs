using PRO.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PRO
{
    public class ParticleManager : MonoScriptBase, ITime_Awake, ITime_Start, ITime_Update
    {
        public static ParticleManager Inst { get; private set; }

        private Dictionary<string, ParticlePool> particlePoolDic = new Dictionary<string, ParticlePool>();
        [NonSerialized]
        public Transform Node;


        public void TimeAwake()
        {
            Inst = this;
            Node = new GameObject("ParticleNode").transform;
            {
                Particle particle = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\Particle_单像素").GetComponent<Particle>();
                particlePoolDic.Add("单像素", new ParticlePool(particle, Node, "单像素"));
            }
            {
                Particle particle = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\Particle_通用0").GetComponent<Particle>();
                particlePoolDic.Add("通用0", new ParticlePool(particle, Node, "通用0"));
            }
            {
                Particle particle = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\Particle_技能播放").GetComponent<Particle>();
                particlePoolDic.Add("技能播放", new ParticlePool(particle, Node, "技能播放"));
            }
        }
        public void TimeStart()
        {
            Node.parent = SceneManager.Inst.PoolNode;
        }

        /// <summary>
        /// 获取一个粒子对象池
        /// 输入ab包内的解析路径 
        /// Assets\ScriptsAssembly\PRO\技能\
        /// Assets\ScriptsAssembly\PRO\Particle\
        /// </summary>
        /// <param name="loadPath"></param>
        /// <returns></returns>
        public ParticlePool GetPool(string loadPath)
        {
            if (particlePoolDic.TryGetValue(loadPath, out ParticlePool pool)) return pool;
            else
            {
                GameObject go = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\技能\{loadPath}");
                if (go == null) go = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\{loadPath}");
                if (go == null) return null;
                Particle particle = go.GetComponent<Particle>();
                pool = new ParticlePool(particle, Node, loadPath);
                particlePoolDic.Add(loadPath, pool);
                return pool;
            }
        }
        public void PutIn(Particle particle)
        {
            GetPool(particle.loadPath).PutIn(particle);
        }

        public void TimeUpdate()
        {
            int time = (int)(TimeManager.deltaTime * 1000);
            for (int i = SceneManager.Inst.NowScene.ActiveParticle.Count - 1; i >= 0; i--)
            {
                var particle = SceneManager.Inst.NowScene.ActiveParticle[i];
                if (particle.Active)
                    particle.UpdateRemainTime(time);
                if (particle.RemainTime <= 0 || particle.Active == false)
                {
                    SceneManager.Inst.NowScene.ActiveParticle.RemoveAt(i);
                    if (particle.RecyleState == false)
                        GetPool(particle.loadPath).PutIn(particle);
                }
            }
        }

        public class ParticlePool
        {
            private MonoObjectPool<Particle> pool;
            public ParticlePool(Particle prefab, Transform parent, string loadPath)
            {
                pool = new MonoObjectPool<Particle>(prefab, parent);
                pool.CreateEvent += (t) => t.Init(loadPath);
            }
            public Particle TakeOut(SceneEntity scene)
            {
                var particle = pool.TakeOut();
                particle.TakeOut(scene);
                particle.SkillPlayAgent?.SetScene(scene);
                scene.ActiveParticle.Add(particle);
                return particle;
            }

            public void PutIn(Particle particle)
            {
                if (particle.loadPath == "通用0")
                {
                    particle.SkillPlayAgent.Skill = null;
                    particle.Renderer.sprite = null;
                }
                particle.PutIn();
                particle.SkillPlayAgent?.SetScene(null);
                pool.PutIn(particle);
            }
        }

        public Particle ToRAM(Proto.ParticleData diskData, SceneEntity screen)
        {
             var particle = GetPool(diskData.LoadPath).TakeOut(screen);
            particle.ToRAM(diskData);
            return particle;
        }
    }
}
