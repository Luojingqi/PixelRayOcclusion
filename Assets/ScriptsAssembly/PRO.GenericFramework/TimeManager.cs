using PRO.DataStructure;
using PRO.Tool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PRO
{
    public class TimeManager : SerializedMonoBehaviour
    {
        public MonoScriptPriority priority;
        public static TimeManager Inst { get; private set; }
        /// <summary>
        /// 上一帧更新花费的时间
        /// </summary>
        public static float deltaTime;
        /// <summary>
        /// 是否启用更新
        /// </summary>
        public static bool enableUpdate = true;
        /// <summary>
        /// 物理多久更新一次
        /// </summary>
        public static float physicsDeltaTime = 0.04f;

        private static float physicsUpdateTime = 0;

        public TMP_Text Text;
        public void Awake()
        {
            Inst = this;
            Text = GameObject.Find("UI/GameMainCanvas/pixel").GetComponent<TMP_Text>();
        }
        private float a = 0;
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                enableUpdate = !enableUpdate;
            }
            a += Time.deltaTime;
            while (a > 0.3f)
            {
                a -= 0.3f;
                Text.text = (1f / Time.deltaTime).ToString();
            }
            if (enableUpdate == false) return;

            ScriptUpdate(Time.deltaTime);
        }

        public void ScriptUpdate(float deltaTime)
        {
            TimeManager.deltaTime = deltaTime;
            for (int i = 0; i < time_Update.Count; i++)
                foreach (var mono in time_Update.FormIndex(i))
                    mono.TimeUpdate();
            for (int i = 0; i < time_LastUpdate.Count; i++)
                foreach (var mono in time_LastUpdate.FormIndex(i))
                    mono.TimeLateUpdate();

            while (addQueue.Count > 0)
                AddMono(addQueue.Dequeue());

            if (initQueue.Count > 0)
            {
                while (initQueue.Count > 0)
                {
                    var mono = initQueue.Dequeue();
                    Debug.Log(mono.GetType().Name + mono.Priority);
                    initQueueTemp.Add(mono);
                    { if (mono is ITime_Awake i) i.TimeAwake(); }
                }
                foreach (var mono in initQueueTemp)
                { if (mono is ITime_Start i) i.TimeStart(); }

                initQueueTemp.Clear();
            }
            while (removeQueue.Count > 0)
                RemoveMono(removeQueue.Dequeue());

            physicsUpdateTime += deltaTime;
            while (physicsUpdateTime > physicsDeltaTime)
            {
                physicsUpdateTime -= physicsDeltaTime;
                Physics2D.Simulate(physicsDeltaTime);
            }
        }


        private SortList<ITime_Update> time_Update = new SortList<ITime_Update>();
        private SortList<ITime_LateUpdate> time_LastUpdate = new SortList<ITime_LateUpdate>();
        private PriorityQueue<MonoScriptBase> initQueue = new PriorityQueue<MonoScriptBase>();
        private List<MonoScriptBase> initQueueTemp = new List<MonoScriptBase>();
        private void AddMono(MonoScriptBase mono)
        {
            if (mono.IsInit == false)
            {
                initQueue.Enqueue(mono, mono.Priority);
                mono.IsInit = true;
            }
            { if (mono is ITime_Update i) time_Update.Add(i, mono.Priority); }
            { if (mono is ITime_LateUpdate i) time_LastUpdate.Add(i, mono.Priority); }
        }
        private void RemoveMono(MonoScriptBase mono)
        {
            { if (mono is ITime_Update i) time_Update.Remove(i, mono.Priority); }
            { if (mono is ITime_LateUpdate i) time_LastUpdate.Remove(i, mono.Priority); }
        }
        private Queue<MonoScriptBase> addQueue = new Queue<MonoScriptBase>();
        private Queue<MonoScriptBase> removeQueue = new Queue<MonoScriptBase>();
        public void MonoQueueAdd(MonoScriptBase mono) => addQueue.Enqueue(mono);
        public void MonoQueueRemove(MonoScriptBase mono) => removeQueue.Enqueue(mono);

    }

    public abstract class MonoScriptBase : SerializedMonoBehaviour, ITime
    {
        [OdinSerialize]
        [ShowInInspector]
        [LabelText("脚本优先级")]
        [GUIColor(0, 1, 0)]
        private int priority = -1;
        public int Priority { get => priority; }
        public bool IsInit { get; set; } = false;

        private void Awake()
        {
            if (priority == -1 && TimeManager.Inst.priority.typeDic.TryGetValue(this.GetType(), out var item))
                priority = item.priority;
            TimeManager.Inst.MonoQueueAdd(this);
        }

        public bool GetActive() => gameObject.activeSelf;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (active) TimeManager.Inst.MonoQueueAdd(this);
            else TimeManager.Inst.MonoQueueRemove(this);
        }
    }


    public interface ITime
    {
        public int Priority { get; }
        public bool IsInit { get; set; }
    }

    public interface ITime_Awake : ITime
    {
        public void TimeAwake();
    }
    public interface ITime_Start : ITime
    {
        public void TimeStart();
    }
    public interface ITime_Update : ITime
    {
        public void TimeUpdate();
    }
    public interface ITime_LateUpdate : ITime
    {
        public void TimeLateUpdate();
    }
}
