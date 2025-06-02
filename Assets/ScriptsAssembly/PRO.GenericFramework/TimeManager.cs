using PRO.DataStructure;
using PRO.Tool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Threading;
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
            Physics2D.simulationMode = SimulationMode2D.Script;
            Text = GameObject.Find("UI/GameMainCanvas/pixel").GetComponent<TMP_Text>();
        }
        private float a = 0;
        private void Update()
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

            if (Monitor.TryEnter(mainThreadUpdateEventLock_UnClear))
            {
                try { event_AddTo_MainThreadUpdateQueue_UnClear?.Invoke(); }
                finally { Monitor.Exit(mainThreadUpdateEventLock_UnClear); }
            }
            if (Monitor.TryEnter(mainThreadUpdateEventLock_Clear))
            {
                try { mainThreadUpdateEvent_Clear?.Invoke(); mainThreadUpdateEvent_Clear = null; }
                finally { Monitor.Exit(mainThreadUpdateEventLock_Clear); }
            }
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



        #region 任务队列与线程锁

        #region 主线程更新事件_UnClear
        private readonly object mainThreadUpdateEventLock_UnClear = new object();
        private event Action event_AddTo_MainThreadUpdateQueue_UnClear;
        /// <summary>
        /// 添加事件到主线程更新队列中，更新完后不被清除
        /// </summary>
        /// <param name="action"></param>
        public void AddToQueue_MainThreadUpdate_UnClear(Action action)
        {
            lock (mainThreadUpdateEventLock_UnClear)
            {
                event_AddTo_MainThreadUpdateQueue_UnClear += action;
            }
        }
        #endregion

        #region 主线程更新事件_Clear

        private readonly object mainThreadUpdateEventLock_Clear = new object();
        private event Action mainThreadUpdateEvent_Clear;
        /// <summary>
        /// 添加事件到主线程更新队列中，更新完后被清除
        /// </summary>
        /// <param name="action"></param>
        public void AddToQueue_MainThreadUpdate_Clear(Action action)
        {
            lock (mainThreadUpdateEventLock_Clear)
            {
                mainThreadUpdateEvent_Clear += action;
            }
        }
        private ObjectPoolArbitrary<AutoResetEvent> resetEventPool = new ObjectPoolArbitrary<AutoResetEvent>(() => new AutoResetEvent(false));
        /// <summary>
        /// 添加事件到主线程更新队列中，更新完后被清除，并等待执行完毕，静止Unity线程调用，会死锁
        /// </summary>
        /// <param name="action"></param>
        public void AddToQueue_MainThreadUpdate_Clear_WaitInvoke(Action action)
        {
            var manual = resetEventPool.TakeOut();
            lock (mainThreadUpdateEventLock_Clear)
            {
                mainThreadUpdateEvent_Clear += action;
                mainThreadUpdateEvent_Clear += () => manual.Set();
            }
            manual.WaitOne();
            resetEventPool.PutIn(manual);
        }
        #endregion

        #endregion
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

    public interface ITime_Awake
    {
        public void TimeAwake();
    }
    public interface ITime_Start
    {
        public void TimeStart();
    }
    public interface ITime_Update
    {
        public void TimeUpdate();
    }
    public interface ITime_LateUpdate
    {
        public void TimeLateUpdate();
    }
}
