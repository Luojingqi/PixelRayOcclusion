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
        public HashSet<Particle> ActiveParticleHash = new HashSet<Particle>();


        private Dictionary<string, GameObjectPool<Particle>> particlePoolDic = new Dictionary<string, GameObjectPool<Particle>>();
        [NonSerialized]
        public Transform Node;


        public void Awake()
        {
            Inst = this;
            Node = new GameObject("ParticleNode").transform;
            Particle particle = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\Particle_单像素").GetComponent<Particle>();
            var pool = new GameObjectPool<Particle>(particle.gameObject, Node);
            pool.CreateEventT += (g, t) => t.Init("单像素");
            pool.TakeOutEventT += (g, t) => t.TakeOut();
            pool.PutInEventT += (g, t) => t.PutIn();
            particlePoolDic.Add("单像素", pool);
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
        public GameObjectPool<Particle> GetPool(string loadPath)
        {
            if (particlePoolDic.TryGetValue(loadPath, out GameObjectPool<Particle> pool)) return pool;
            else
            {
                GameObject go = null;
                go = go = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\GamePlay\技能\{loadPath}");
                if (go == null) go = AssetManager.Load_A<GameObject>("particle.ab", @$"ScriptsAssembly\PRO\Particle\{loadPath}");
                if (go == null) return null;
                Particle particle = go.GetComponent<Particle>();
                pool = new GameObjectPool<Particle>(particle.gameObject, Node);
                pool.CreateEventT += (g, t) => t.Init(loadPath);
                pool.TakeOutEventT += (g, t) => t.TakeOut();
                pool.PutInEventT += (g, t) => t.PutIn();
                particlePoolDic.Add(loadPath, pool);
                return pool;
            }
        }

        private List<Particle> ReadyPutInList = new List<Particle>();
        public void Update()
        {
            int time = (int)(Time.deltaTime * 1000);
            foreach (var particle in ActiveParticleHash)
            {
                particle.UpdateRemainTime(time);
                if (particle.remainTime <= 0)
                    ReadyPutInList.Add(particle);
            }
            foreach (var particle in ReadyPutInList)
            {
                GetPool(particle.loadPath).PutIn(particle.gameObject);
            }
            ReadyPutInList.Clear();
        }


    }
}
