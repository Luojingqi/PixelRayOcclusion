using Google.FlatBuffers;
using PRO.Skill;
using PRO.Tool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.SkillEditor
{
    /// <summary>
    /// 技能播放的执行人
    /// </summary>
    public class SkillPlayAgent : MonoScriptBase, IScene, ITime_Update
    {
        public SceneEntity Scene => _scene;
        private SceneEntity _scene;
        public void SetScene(SceneEntity scene) => _scene = scene;
        public void Init()
        {
            if (AgentSprice == null)
                AgentSprice = transform.GetComponent<SpriteRenderer>();
        }
        [LabelText("动画轨道精灵渲染器")]
        public SpriteRenderer AgentSprice;
        /// <summary>
        /// 特效轨道的精灵图渲染器
        /// </summary>
        public List<SpriteRenderer> SpecialEffect2DSpriteList = new List<SpriteRenderer>();


        [ShowInInspector]
        /// <summary>
        /// 播放，暂停播放技能,
        /// </summary>
        public bool Play
        {
            get { return play; }
            set { play = value; }
        }
        private bool play = false;

        public void AddSkill(SkillPlayData playData) => SkillPlayDataSet.Add(playData);
        public bool RemoveSkill(SkillPlayData playData) => SkillPlayDataSet.Remove(playData);

        public void TimeUpdate()
        {
            if (Play == false || SkillPlayDataSet.Count == 0) return;
            int deltaTime = (int)(TimeManager.deltaTime * 1000);
            foreach (var data in SkillPlayDataSet)
                if (data.UpdateFrameScript(this, deltaTime))
                    tempSkillPlayDataList.Add(data);
            for (int i = 0; i < tempSkillPlayDataList.Count; i++)
            {
                var data = tempSkillPlayDataList[i];
                SkillPlayDataSet.Remove(data);
                SkillPlayData.PutIn(data);
            }
            tempSkillPlayDataList.Clear();
        }
        private List<SkillPlayData> tempSkillPlayDataList = new();
        [ShowInInspector]
        private HashSet<SkillPlayData> SkillPlayDataSet = new();



        public Offset<Flat.SkillPlayerAgentData> ToDisk(FlatBufferBuilder builder)
        {
            Span<int> datasOffsetArray = stackalloc int[SkillPlayDataSet.Count];
            int dataIndex = 0;
            foreach (var data in SkillPlayDataSet)
                datasOffsetArray[dataIndex++] = data.ToDisk(builder).Value;
            var datasOffsetArrayOffset = builder.CreateVector_Offset(datasOffsetArray);

            Flat.SkillPlayerAgentData.StartSkillPlayerAgentData(builder);
            Flat.SkillPlayerAgentData.AddPlay(builder, play);
            Flat.SkillPlayerAgentData.AddDatas(builder, datasOffsetArrayOffset);
            return Flat.SkillPlayerAgentData.EndSkillPlayerAgentData(builder);
        }
        public void ToRAM(Flat.SkillPlayerAgentData diskData)
        {
            play = diskData.Play;
            for (int i = diskData.DatasLength - 1; i >= 0; i--)
            {
                var data = SkillPlayData.TakeOut();
                data.ToRAM(diskData.Datas(i).Value);
                SkillPlayDataSet.Add(data);
            }
        }
    }
    public class SkillPlayData
    {
        public SkillVisual_Disk SkillVisual;
        public List<SkillLogicBase> SkillLogicList = new List<SkillLogicBase>(4);
        public int time;
        public int nowFrame;
        public int playTrack = ~0;
        private bool firstUpdate = true;
        private bool firstEnd = true;
        public bool UpdateFrameScript(SkillPlayAgent agent, float deltaTime) => UpdateFrameScript(agent, (int)(deltaTime * 1000));
        public bool UpdateFrameScript(SkillPlayAgent agent, int deltaTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                for (int i = 0; i < SkillLogicList.Count; i++)
                    SkillLogicList[i].Before_SkillPlay(SkillVisual);
                SkillVisual.UpdateFrame(agent, this);
            }
            time += deltaTime;
            while (time >= SkillVisual.FrameTime)
            {
                nowFrame++;
                time -= SkillVisual.FrameTime;
                if (nowFrame >= SkillVisual.MaxFrame)
                {
                    if (firstEnd)
                    {
                        firstEnd = false;
                        for (int i = 0; i < SkillLogicList.Count; i++)
                            SkillLogicList[i].Before_SkillPlay(SkillVisual);
                    }
                    return true;
                }
                SkillVisual.UpdateFrame(agent, this);
            }
            return false;
        }
        public void ResetFrameIndex()
        {
            time = 0;
            nowFrame = 0;
            firstUpdate = true;
            firstEnd = true;
        }

        private static ObjectPool<SkillPlayData> pool = new ObjectPool<SkillPlayData>();
        public static SkillPlayData TakeOut() => pool.TakeOut();
        public static void PutIn(SkillPlayData data)
        {
            for (int i = 0; i < data.SkillLogicList.Count; i++)
                data.SkillLogicList[i].After_SkillPlay(data.SkillVisual);
            data.SkillLogicList.Clear();
            data.SkillVisual = null;
            data.time = 0;
            data.nowFrame = 0;
            data.playTrack = ~0;
            data.firstUpdate = true;
            data.firstEnd = true;
        }

        public Offset<Flat.SkillplayerDataData> ToDisk(FlatBufferBuilder builder)
        {
            var visualPathOffset = builder.CreateString(SkillVisual.loadPath);
            Span<int> logicGuidOffsetArray = stackalloc int[SkillLogicList.Count];
            for (int i = 0; i < SkillLogicList.Count; i++)
                logicGuidOffsetArray[i] = builder.CreateString(SkillLogicList[i].GUID).Value;
            var logicGuidOffsetArrayOffset = builder.CreateVector_Offset(logicGuidOffsetArray);
            Flat.SkillplayerDataData.StartSkillplayerDataData(builder);
            Flat.SkillplayerDataData.AddSkillVisualPath(builder, visualPathOffset);
            Flat.SkillplayerDataData.AddSkillLogicGuidList(builder, logicGuidOffsetArrayOffset);
            Flat.SkillplayerDataData.AddTime(builder, time);
            Flat.SkillplayerDataData.AddNowFrame(builder, nowFrame);
            return Flat.SkillplayerDataData.EndSkillplayerDataData(builder);
        }
        public void ToRAM(Flat.SkillplayerDataData diskData)
        {
            SkillVisual = AssetManagerEX.LoadSkillVisualDisk(diskData.SkillVisualPath);
            for (int i = diskData.SkillLogicGuidListLength - 1; i >= 0; i--)
            {

            }
            time = diskData.Time;
            nowFrame = diskData.NowFrame;
        }
    }
}
