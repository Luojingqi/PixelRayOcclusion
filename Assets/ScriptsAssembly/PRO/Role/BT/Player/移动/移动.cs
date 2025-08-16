//#define 寻路路径显示
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NodeCanvas.Framework;
using PRO.DataStructure;
using PRO.Skill;
using PRO.SkillEditor;
using System.Collections.Generic;
using UnityEngine;
namespace PRO.BT.移动
{
    public class 移动 : ActionTask
    {
        public class Data
        {
            public PriorityQueue<Nav.Node> queue = new();
            public Dictionary<Nav.Node, Nav.Node> dic = new();
            public List<Nav.Node> navList = new(36);
            public int index;
            public float deltaTime;
            public float oldGravityScale;
            public TweenerCore<Vector3, Vector3, VectorOptions> doTweener;
#if 寻路路径显示
            public List<Particle> list = new();
#endif
            public void Clear()
            {
                queue.Clear(); dic.Clear(); navList.Clear();
                index = 0;
                deltaTime = 0;
                oldGravityScale = 0;
                doTweener = null;
                #region 移动路径回收
#if 寻路路径显示
                var pool = ParticleManager.Inst.GetPool("单像素");
                for (int i = 0; i < list.Count; i++)
                    pool.PutIn(list[i]);
                list.Clear();
#endif
                #endregion
            }
        }
        private Data data = new Data();
        private static float jumpOutDistance = Pixel.Size * 2;


        public BBParameter<Role> Agent;
        public BBParameter<Vector2Int> 移动目标;

        public SkillVisual_Disk SkillVisual_移动;
        private SkillPlayData playData = new();
        protected override string OnInit()
        {
            var agent = Agent.value;
            agent.Info.移动速度.Value_基础 = 5;
            playData.SkillVisual = SkillVisual_移动;
            playData.SkillLogicList.Add(new SkillLogic_移动(null));
            return base.OnInit();
        }

        protected override void OnExecute()
        {
            var agent = Agent.value;
            data.oldGravityScale = agent.Rig2D.gravityScale;
            if (Mathf.Abs(agent.Rig2D.velocity.sqrMagnitude) > 0.01f) { EndAction(false); return; }
            if (agent.GlobalPos == 移动目标.value) { EndAction(false); return; }
            Nav.TryNav(agent.Scene, agent.Info.NavMould, 5, 5, agent.GlobalPos, 移动目标.value, data.queue, data.dic, data.navList);
            if (data.navList.Count > 1)
            {
                #region 移动路线显示
#if 寻路路径显示
                var pool = ParticleManager.Inst.GetPool("单像素");
                for (int i = 0; i < data.navList.Count; i++)
                {
                    var p = pool.TakeOut(agent.Scene);
                    p.Rig2D.simulated = false;
                    data.list.Add(p);
                    p.SetGlobal(data.navList[i].globalPos);
                    if (data.navList[i].fallingHeight != 0)
                        p.Renderer.color = Color.blue;
                }
#endif
                #endregion
                agent.Rig2D.gravityScale = 0;
            }
            else
            {
                EndAction(false); return;
            }
        }
        protected override void OnUpdate()
        {
            var agent = Agent.value;
            playData.UpdateFrameScript(agent.SkillPlayAgent, TimeManager.deltaTime);
            if (data.deltaTime <= 0)
            {
                data.index++;
                if (data.navList.Count > data.index && data.navList[data.index].fallingHeight == 0)
                {
                    data.deltaTime = (float)(1 / agent.Info.移动速度.Value);
                    data.doTweener = agent.transform.DOMove(Block.GlobalToWorld(data.navList[data.index].globalPos) + new Vector3(Pixel.Size_Half, 0), data.deltaTime);
                }
                else
                {
                    EndAction(true); return;
                }
            }
            else
            {
                if (Vector2.Distance(agent.transform.position, (Vector2)Block.GlobalToWorld(data.navList[data.index - 1].globalPos) + new Vector2(Pixel.Size_Half, 0)) > jumpOutDistance
                    && Vector2.Distance(agent.transform.position, (Vector2)Block.GlobalToWorld(data.navList[data.index].globalPos) + new Vector2(Pixel.Size_Half, 0)) > jumpOutDistance)
                {
                    EndAction(false); return;
                }
            }
            data.deltaTime -= TimeManager.deltaTime;
        }
        protected override void OnStop()
        {
            var agent = Agent.value;
            playData.ResetFrameIndex(agent.SkillPlayAgent);
            agent.Rig2D.gravityScale = data.oldGravityScale;
            if (data.doTweener != null)
                data.doTweener.Kill();
            data.Clear();
        }


        public class SkillLogic_移动 : SkillLogicBase
        {
            public SkillLogic_移动(string guid) : base(guid) { }

            private Quaternion startRotation;

            public override void Before_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual)
            {
                startRotation = agent.transform.rotation;
            }

            private TweenerCore<Quaternion, Quaternion, NoOptions> doTween;
            public override void Before_SpecialEffectSlice2D(SkillPlayAgent agent, SkillPlayData playData, SpecialEffectSlice2D_Disk slice, FrameData frameDataa)
            {
                doTween = agent.transform.DOLocalRotateQuaternion(slice.rotation, playData.SkillVisual.FrameTime / 1000f);
            }

            public override void After_SkillPlay(SkillPlayAgent agent, SkillPlayData playData, SkillVisual_Disk skillVisual)
            {
                if (doTween != null)
                    doTween.Kill();
                agent.transform.DOLocalRotateQuaternion(startRotation, playData.SkillVisual.FrameTime / 1000f / 2);
                startRotation = Quaternion.identity;
            }
        }
    }
}