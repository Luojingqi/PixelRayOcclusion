using PRO.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PRO
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Inst { get; private set; }



        private Dictionary<string, ParticlePool> particlePoolDic = new Dictionary<string, ParticlePool>();
        [NonSerialized]
        public Transform Node;


        public void Awake()
        {
            Inst = this;
            Node = new GameObject("ParticleNode").transform;
            {
                Particle particle = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\Particle_单像素").GetComponent<Particle>();
                particlePoolDic.Add("单像素", new ParticlePool(particle.gameObject, Node, "单像素"));
            }
            {
                Particle particle = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\Particle_通用0").GetComponent<Particle>();
                particlePoolDic.Add("通用0", new ParticlePool(particle.gameObject, Node, "通用0"));
            }
        }
        public void Start()
        {
            Node.parent = SceneManager.Inst.PoolNode;
        }

        /// <summary>
        /// 获取一个粒子对象池
        /// 输入ab包内的解析路径 
        /// Assets\ScriptsAssembly\GamePlay\技能\
        /// Assets\ScriptsAssembly\PRO\Particle\
        /// </summary>
        /// <param name="loadPath"></param>
        /// <returns></returns>
        public ParticlePool GetPool(string loadPath)
        {
            if (particlePoolDic.TryGetValue(loadPath, out ParticlePool pool)) return pool;
            else
            {
                GameObject go = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\GamePlay\技能\{loadPath}");
                if (go == null) go = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\{loadPath}");
                if (go == null) return null;
                Particle particle = go.GetComponent<Particle>();
                pool = new ParticlePool(particle.gameObject, Node, loadPath);
                particlePoolDic.Add(loadPath, pool);
                return pool;
            }
        }

        private List<Particle> ReadyPutInList = new List<Particle>();
        public void Update()
        {
            int time = (int)(Time.deltaTime * 1000);
            foreach (var particle in SceneManager.Inst.NowScene.ActiveParticleHash)
            {
                if (particle.Active)
                    particle.UpdateRemainTime(time);
                if (particle.RemainTime <= 0 || particle.Active == false)
                    ReadyPutInList.Add(particle);
            }
            foreach (var particle in ReadyPutInList)
            {
                SceneManager.Inst.NowScene.ActiveParticleHash.Remove(particle);
                if (particle.RecyleState == false)
                    GetPool(particle.loadPath).PutIn(particle);
            }
            ReadyPutInList.Clear();
        }

        public class ParticlePool
        {
            private GameObjectPool<Particle> pool;
            public ParticlePool(GameObject prefab, Transform parent, string loadPath)
            {
                pool = new GameObjectPool<Particle>(prefab, parent);
                pool.CreateEventT += (g, t) => t.Init(loadPath);
            }
            public Particle TakeOut(SceneEntity scene)
            {
                var particle = pool.TakeOutT();
                particle.TakeOut(scene);
                particle.SkillPlayAgent?.SetScene(scene);
                scene.ActiveParticleHash.Add(particle);
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
                pool.PutIn(particle.gameObject);
            }
        }
    }
}
