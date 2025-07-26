using Google.FlatBuffers;
using PRO.AI;
using PRO.Skill;
using PRO.TurnBased;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using PRO.AI.Flat;
namespace PRO
{
    public class GamePlayMain : MonoScriptBase, ITime_Awake, ITime_Update
    {
        public static GamePlayMain Inst { get; private set; }

        public void TimeAwake()
        {
            Inst = this;
            OperateFSMBase.InitOperateType();
            MCTS.Init();
            mcts = new MCTS();
        }
        [Button]
        public void BBBB()
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            int length = 2;
            Span<Base> bases = stackalloc Base[length];
            Span<int> AOffsetArray = stackalloc int[length];
            //Span<int> BOffsetArray = stackalloc int[length];
            for (int i = 0; i < length; i++)
            {
                A.StartA(builder);
                A.AddValue(builder, i);
                AOffsetArray[i] = A.EndA(builder).Value;

                bases[i] = Base.A;
                //var offset = builder.CreateString($"{i}_{i}");
                // B.StartB(builder);
                // B.AddValue(builder, offset);
                //BOffsetArray[i] =  B.EndB(builder).Value;
            }
            var AOffsetArrayOffset = builder.CreateVector_Offset(AOffsetArray);
            var typeOffset = builder.CreateVector_Data(bases);
            Test.StartTest(builder);
            Test.AddValuesType(builder, typeOffset);
            Test.AddValues(builder, AOffsetArrayOffset);
            builder.Finish(Test.EndTest(builder).Value);

            var diskData = Test.GetRootAsTest(builder.DataBuffer);
            for (int i = diskData.ValuesLength - 1; i >= 0; i--)
                Debug.Log(diskData.ValuesType(i) + "|" + diskData.Values<A>(i).Value.Value);
        }


        public RoundFSM Round
        {
            get => round;
            set
            {
                round = value;
#if PRO_RENDER
                if (round == null)
                {
                    GameMainUIC.Inst.CloseTurnUI();
                }
                else
                {
                    GameMainUIC.Inst.OpenTurnUI();
                    GameMainUIC.Inst.SetTurn(round.State3_Turn.TurnFSMList, round.State3_Turn.NowTurn.Index);
                }
#endif
            }
        }
        [ShowInInspector]
        private RoundFSM round;

        private MCTS mcts;

        bool p = false;

        public void TimeUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (p) Time.timeScale = 1;
                else Time.timeScale = 0;
                p = !p;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                var round = new RoundFSM(SceneManager.Inst.NowScene, System.Guid.NewGuid().ToString());
                var round_state0 = round.GetState<RoundState0_DataReady>();
                for (int i = 0; i < 2; i++)
                {
                    var role = RoleManager.Inst.TakeOut("默认", SceneManager.Inst.NowScene, System.Guid.NewGuid().ToString());
                    role.transform.position = new Vector3(i / 0.5f, 0);
                    round_state0.AddRole(role);

                    role.AddOperate(typeof(Skill_0_0));
                    role.AddOperate(typeof(Skill_3_3));
                }

                round_state0.ReadyOver();
                Round = round;
            }

#if PRO_MCTS_SERVER
            if (Input.GetKeyDown(KeyCode.M) && m == false)
            {
                MTCS();
                m = true;
            }
        }
        private bool m = false;

        [Button]
        public void MTCS()
        {
            mcts.开始模拟(round);
        }
#elif PRO_MCTS_CLIENT
        }
#endif
    }
}