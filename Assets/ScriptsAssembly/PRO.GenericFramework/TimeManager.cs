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
        public static float deltaTime { get; private set; }
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
            if (enableUpdate)
                ScriptUpdate(Time.deltaTime);



            #region 主线程事件轮训
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
            #endregion
        }

        public void ScriptUpdate(float deltaTime)
        {
            TimeManager.deltaTime = deltaTime;

            physicsUpdateTime += deltaTime;
            while (physicsUpdateTime > physicsDeltaTime)
            {
                physicsUpdateTime -= physicsDeltaTime;
                Physics2D.Simulate(physicsDeltaTime);
            }

            for (int i = 0; i < time_Update.Count; i++)
                foreach (var mono in time_Update.FormIndex(i))
                    if (mono.isActiveAndEnabled)
                        mono.TimeUpdate();
            for (int i = 0; i < time_LastUpdate.Count; i++)
                foreach (var mono in time_LastUpdate.FormIndex(i))
                    if (mono.isActiveAndEnabled)
                        mono.TimeLateUpdate();

            #region 添加mono组件
            while (addQueue.Count > 0)
            {
                var mono = addQueue.Dequeue();
                if (mono.isActiveAndEnabled == false) continue;
                if (mono.IsInit == false)
                {
                    initAwakeQueue.Enqueue(mono, mono.Priority);
                    mono.IsInit = true;
                }
                { if (mono is ITime_Update i) time_Update.Add(i, mono.Priority); }
                { if (mono is ITime_LateUpdate i) time_LastUpdate.Add(i, mono.Priority); }
            }
            #endregion
            #region 初始化mono组件
            if (initAwakeQueue.Count > 0)
            {
                while (initAwakeQueue.Count > 0)
                {
                    var mono = initAwakeQueue.Dequeue();
                    if (mono.isActiveAndEnabled == false) continue;
                    initStartList.Add(mono);
                    { if (mono is ITime_Awake i) i.TimeAwake(); }
                }
                foreach (var mono in initStartList)
                { if (mono is ITime_Start i) i.TimeStart(); }

                initStartList.Clear();
            }
            #endregion
            #region 移除mono组件
            while (removeQueue.Count > 0)
            {
                var mono = removeQueue.Dequeue();
                if (mono.isActiveAndEnabled == true) continue;
                { if (mono is ITime_Update i) time_Update.Remove(i, mono.Priority); }
                { if (mono is ITime_LateUpdate i) time_LastUpdate.Remove(i, mono.Priority); }
            }
            #endregion
        }


        private SortList<ITime_Update> time_Update = new SortList<ITime_Update>(20);
        private SortList<ITime_LateUpdate> time_LastUpdate = new SortList<ITime_LateUpdate>(20);


        private PriorityQueue<MonoScriptBase> initAwakeQueue = new PriorityQueue<MonoScriptBase>(20);
        private List<MonoScriptBase> initStartList = new List<MonoScriptBase>();


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
                mainThreadUpdateEvent_Clear += () => { action(); manual.Set(); };
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
        }

        private void OnEnable()
        {
            TimeManager.Inst.MonoQueueAdd(this);
        }
        private void OnDisable()
        {
            TimeManager.Inst.MonoQueueRemove(this);
        }
    }


    public interface ITime
    {
        public int Priority { get; }
        public bool IsInit { get; set; }

        public bool isActiveAndEnabled { get; }
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
