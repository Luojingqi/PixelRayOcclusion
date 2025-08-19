using Google.FlatBuffers;
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
        [LabelText("特效轨道的精灵图渲染器")]
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
        [Button]
        private void AddSkill(SkillPlayData playData) => SkillPlayDataList.Add(playData);

        public void TimeUpdate()
        {
            if (Play == false || SkillPlayDataList.Count == 0) return;
            for (int i = 0; i < SkillPlayDataList.Count; i++)
            {
                var data = SkillPlayDataList[i];
                if (data.UpdateFrameScript(this, TimeManager.deltaTime))
                {
                    SkillPlayDataList.RemoveAt(i);
                    i--;
                    SkillPlayData.PutIn(data);
                }
            }
        }

        public List<SkillPlayData> SkillPlayDataList = new();

        public Offset<Flat.SkillPlayerAgentData> ToDisk(FlatBufferBuilder builder)
        {
            Span<int> datasOffsetArray = stackalloc int[SkillPlayDataList.Count];
            int dataIndex = 0;
            foreach (var data in SkillPlayDataList)
                datasOffsetArray[dataIndex++] = data.ToDisk(builder).Value;
            var datasOffsetArrayOffset = builder.CreateVector_Offset(datasOffsetArray);

            Flat.SkillPlayerAgentData.StartSkillPlayerAgentData(builder);
            Flat.SkillPlayerAgentData.AddPlay(builder, play);
            Flat.SkillPlayerAgentData.AddDataList(builder, datasOffsetArrayOffset);
            return Flat.SkillPlayerAgentData.EndSkillPlayerAgentData(builder);
        }
        public void ToRAM(Flat.SkillPlayerAgentData diskData)
        {
            play = diskData.Play;
            for (int i = diskData.DataListLength - 1; i >= 0; i--)
            {
                var data = SkillPlayData.TakeOut();
                data.ToRAM(diskData.DataList(i).Value);
                SkillPlayDataList.Add(data);
            }
        }
    }
    public class SkillPlayData
    {
        public SkillVisual_Disk SkillVisual;
        public List<SkillLogicBase> SkillLogicList = new List<SkillLogicBase>(4);
        public float time;
        public int nowFrame;
        public int playTrack = ~0;
        private bool firstUpdate = true;
        private bool firstEnd = true;

        public bool UpdateFrameScript(SkillPlayAgent agent, float deltaTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                for (int i = 0; i < SkillLogicList.Count; i++)
                    SkillLogicList[i].Before_SkillPlay(agent, this, SkillVisual);
                SkillVisual.UpdateFrame(agent, this);
            }
            time += deltaTime;
            while (time >= SkillVisual.FrameTime)
            {
                //更新到下一帧前，调用上一帧的最后一次补间，不然可能会直接越过导致补间时间不够
                for (int trackIndex = 0; trackIndex < SkillVisual.EventTrackList.Count; trackIndex++)
                {
                    var track = SkillVisual.EventTrackList[trackIndex];
                    var slice = track.SlickArray[nowFrame];
                    if (slice == null || slice.enable == false) continue;
                    var eventDisk = slice as EventDisk_Base;
                    var sliceFrame = nowFrame - eventDisk.startFrame;
                    for (int i = 0; i < SkillLogicList.Count; i++)
                        SkillLogicList[i].Update_Event(agent, this, eventDisk, new FrameData(nowFrame, sliceFrame, track.TrackEnum, trackIndex), SkillVisual.FrameTime - (time - deltaTime), (sliceFrame + 1) * SkillVisual.FrameTime);
                }
                SkillVisual.UpdateEndFrame(agent, this);

                nowFrame++;
                time -= SkillVisual.FrameTime;
                if (nowFrame >= SkillVisual.MaxFrame)
                {
                    if (firstEnd)
                    {
                        firstEnd = false;
                        for (int i = 0; i < SkillLogicList.Count; i++)
                            SkillLogicList[i].After_SkillPlay(agent, this, SkillVisual);
                    }
                    return true;
                }
                SkillVisual.UpdateFrame(agent, this);
            }
            for (int trackIndex = 0; trackIndex < SkillVisual.EventTrackList.Count; trackIndex++)
            {
                var track = SkillVisual.EventTrackList[trackIndex];
                var slice = track.SlickArray[nowFrame];
                if (slice == null || slice.enable == false) continue;
                var eventDisk = slice as EventDisk_Base;
                var sliceFrame = nowFrame - eventDisk.startFrame;
                for (int i = 0; i < SkillLogicList.Count; i++)
                    SkillLogicList[i].Update_Event(agent, this, eventDisk, new FrameData(nowFrame, sliceFrame, track.TrackEnum, trackIndex), deltaTime, time + sliceFrame * SkillVisual.FrameTime);
            }
            return false;
        }
        public void ResetFrameIndex(SkillPlayAgent agent)
        {
            if (firstUpdate == false && firstEnd)
                for (int i = 0; i < SkillLogicList.Count; i++)
                    SkillLogicList[i].After_SkillPlay(agent, this, SkillVisual);
            time = 0;
            nowFrame = 0;
            firstUpdate = true;
            firstEnd = true;
        }

        private static ObjectPool<SkillPlayData> pool = new ObjectPool<SkillPlayData>();
        public static SkillPlayData TakeOut() => pool.TakeOut();
        public static void PutIn(SkillPlayData playData)
        {
            playData.SkillLogicList.Clear();
            playData.SkillVisual = null;
            playData.time = 0;
            playData.nowFrame = 0;
            playData.playTrack = ~0;
            playData.firstUpdate = true;
            playData.firstEnd = true;
        }

        public Offset<Flat.SkillPlayerDataData> ToDisk(FlatBufferBuilder builder)
        {
            var visualPathOffset = builder.CreateString(SkillVisual.loadPath);
            Span<int> logicOffsetArray = stackalloc int[SkillLogicList.Count];
            for (int i = 0; i < SkillLogicList.Count; i++)
                logicOffsetArray[i] = SkillLogicList[i].ToDisk(builder).Value;
            var logicOffsetArrayOffset = builder.CreateVector_Offset(logicOffsetArray);
            Flat.SkillPlayerDataData.StartSkillPlayerDataData(builder);
            Flat.SkillPlayerDataData.AddSkillVisualPath(builder, visualPathOffset);
            Flat.SkillPlayerDataData.AddSkillLogicList(builder, logicOffsetArrayOffset);
            Flat.SkillPlayerDataData.AddTime(builder, time);
            Flat.SkillPlayerDataData.AddNowFrame(builder, nowFrame);
            return Flat.SkillPlayerDataData.EndSkillPlayerDataData(builder);
        }
        public void ToRAM(Flat.SkillPlayerDataData diskData)
        {
            SkillVisual = AssetManagerEX.LoadSkillVisualDisk(diskData.SkillVisualPath);
            for (int i = diskData.SkillLogicListLength - 1; i >= 0; i--)
            {
                var logicDiskData = diskData.SkillLogicList(i).Value;
                var logic = SkillLogicBase.CreateSkillLogic(logicDiskData.Type);
                logic.ToRAM(logicDiskData);
                SkillLogicList.Add(logic);
            }
            time = diskData.Time;
            nowFrame = diskData.NowFrame;
        }
    }
}
