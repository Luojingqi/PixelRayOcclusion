using Cysharp.Threading.Tasks;
using Google.FlatBuffers;
using PRO.Buff.Base;
using PRO.Flat.Ex;
using PRO.Skill;
using PRO.SkillEditor;
using PRO.Tool;
using PRO.TurnBased;
using PROTool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
namespace PRO
{
    public class Role : MonoScriptBase, IScene, ITime_Update
    {
        public SpriteRenderer SpriteRenderer { get; private set; }
        public SkillPlayAgent SkillPlayAgent { get; private set; }

        public Rigidbody2D Rig2D { get; private set; }


        [HideInInspector]
        public FreelyLightSource source;
        [HideInInspector]
        public Nav nav;

        public SceneEntity Scene => _scene;
        private SceneEntity _scene;

        [OdinSerialize]
        private Toward toward = Toward.right;

        public Toward Toward
        {
            get { return toward; }
            set
            {
                if (toward == value) return;
                toward = value;
                SpriteRenderer.flipX = !SpriteRenderer.flipX;
            }
        }
        public string RoleTypeName;
        public string GUID => guid;
        private string guid;
        public string Name;
        public Sprite Icon;
        [NonSerialized]
        public RoleInfo Info = new RoleInfo();

        public TurnFSM Turn;
        public void Init()
        {
            for (int i = 0; i < AllBuff.Length; i++)
                AllBuff[i] = new SortList<BuffBase>(8);
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SkillPlayAgent = GetComponent<SkillPlayAgent>();
            SkillPlayAgent.Init();

            Rig2D = GetComponent<Rigidbody2D>();

            //CanUseOperateList.Add(new Skill_1_0());
            //CanUseOperateList.Add(new Skill_1_1());
            //CanUseOperateList.Add(new Skill_3_0());
            //string skillGuid = System.Guid.NewGuid().ToString();
            //AllCanUseOperate.Add(skillGuid, new Skill_3_3(skillGuid));
            //CanUseOperateList.Add(new Skill_3_4());
            //CanUseOperateList.Add(new Skill_3_5());
            //CanUseOperateList.Add(new Skill_3_6());
            //CanUseOperateList.Add(new Skill_3_7());
            //CanUseOperateList.Add(new Skill_5_0());
            //CanUseOperateList.Add(new Skill_5_1());
            //CanUseOperateList.Add(new Skill_5_2());
            //CanUseOperateList.Add(new Skill_5_3());
            //CanUseOperateList.Add(new Skill_5_4());
            //CanUseOperateList.Add(new Skill_5_5());
            //CanUseOperateList.Add(new Skill_5_6());
            //CanUseOperateList.Add(new Skill_5_7());
            //CanUseOperateList.Add(new Skill_5_8());
            //CanUseOperateList.Add(new Skill_5_9());
            //CanUseOperateList.Add(new Skill_6_0());
            //CanUseOperateList.Add(new Skill_6_1());
            //CanUseOperateList.Add(new Skill_6_2());
            //CanUseOperateList.Add(new Skill_6_6());
            //CanUseOperateList.Add(new Skill_7_0());
            //CanUseOperateList.Add(new Skill_7_1());
            //CanUseOperateList.Add(new Skill_8_0());
            //AddBuff(new Buff_1_0());
            //AddBuff(new Buff_2_0());
            //AddBuff(new Buff_2_1());
            //AddBuff(new Buff_2_3());
            //AddBuff(new Buff_2_5());
            //AddBuff(new Buff_2_7());
            ////AddBuff(new Buff_2_8());
            //AddBuff(new Buff_2_9());
            //AddBuff(new Buff_2_10());
            //AddBuff(new Buff_2_11());
        }
        public void TakeOut(SceneEntity scene, string guid)
        {
            this.guid = guid;
            _scene = scene;
            source = FreelyLightSource.New(_scene, new Color32(200, 200, 200, 255), 15);
            SkillPlayAgent.SetScene(_scene);
        }
        public void PutIn()
        {
            guid = null;
            source.GloabPos = null;
            source = null;
            _scene = null;
            SkillPlayAgent.SetScene(null);
            AllCanUseOperate.Clear();
        }
        public void TimeUpdate()
        {
            source.GloabPos = Block.WorldToGlobal(transform.position) + nav.AgentMould.center;
            for (int i = 0; i < (int)BuffTriggerType.end; i++)
            {
                var buffTriggerList = AllBuff[i];
                for (int j = 0; j < buffTriggerList.Count; j++)
                {
                    var list = new ReusableList<BuffBase>();
                    foreach (var buff in buffTriggerList.FormIndex(i))
                        list.Add(buff);
                    for (int k = 0; k < list.Count; k++)
                        list[k].Update();
                    list.Dispose();
                }
            }
        }
        #region 选择方法等
        public async UniTask Play被攻击Animation()
        {
            SpriteRenderer.color = Color.red;
            await UniTask.Delay(150);
            SpriteRenderer.color = Color.white;
        }
        public async UniTask Play被治疗Animation()
        {
            SpriteRenderer.color = Color.green;
            await UniTask.Delay(150);
            SpriteRenderer.color = Color.white;
        }
        public void Select()
        {
            SpriteRenderer.color = Color.yellow;
        }
        public void UnSelect()
        {
            SpriteRenderer.color = Color.white;
        }
        public void LookAt(Vector2 look)
        {
            if (look.x < transform.position.x) Toward = Toward.left;
            else Toward = Toward.right;
        }
        [ShowInInspector]
        public Vector2Int GlobalPos { get => Block.WorldToGlobal(transform.position); set => transform.position = Block.GlobalToWorld(value); }
        public Vector2Int CenterPos => GlobalPos + nav.AgentMould.center;
        #endregion

        #region operate
        [NonSerialized]
        [ShowInInspector]
        /// <summary>
        /// key：guid value：skill
        /// </summary>
        public Dictionary<string, OperateFSMBase> AllCanUseOperate = new Dictionary<string, OperateFSMBase>();

        [Button]
        public void AddOperate([ValueDropdown(nameof(DropdownOperateTypeName))] Type type)
        {
            var operate = OperateFSMBase.CreateOperate(type.Name, System.Guid.NewGuid().ToString());
            AllCanUseOperate.Add(operate.GUID, operate);
            operate.Agent = this;
            if (Turn.RoundFSM == GamePlayMain.Inst.Round && Turn == Turn.RoundFSM.State3_Turn.NowTurn)
                GamePlayMain.Inst.Round = GamePlayMain.Inst.Round;
        }

        private List<Type> DropdownOperateTypeName() => ReflectionTool.GetDerivedClasses(typeof(OperateFSMBase));
        #endregion

        #region buff
        public SortList<BuffBase>[] AllBuff { get; } = new SortList<BuffBase>[(int)BuffTriggerType.end];
        /// <summary>
        /// 独有的buff
        /// </summary>
        public Dictionary<Type, ReusableList<BuffBase>> BuffTypeDic { get; } = new Dictionary<Type, ReusableList<BuffBase>>();
        public void UpdateBuffUI()
        {
            if (GameMainUIC.Inst.NowShowRole != this) return;
            GameMainUIC.Inst.UpdateNowShowRoleBuff();
        }

        public void ForEachBuffApplyEffect(BuffTriggerType type, CombatContext context, int byAgentIndex)
        {
            var buffList = AllBuff[(int)type];
            for (int i = 0; i < buffList.Count; i++)
                foreach (var buff in buffList.FormIndex(i))
                    buff.ApplyEffect(context, byAgentIndex);
        }
        #endregion

        #region 序列化与反序列化
        public Offset<Flat.RoleData> ToDisk(FlatBufferBuilder builder)
        {
            var skillPlayAgentOffset = SkillPlayAgent.ToDisk(builder);
            var navTypeOffset = builder.CreateString(nav?.TypeName);
            var roleTypeOffset = builder.CreateString(RoleTypeName);
            var guidOffset = builder.CreateString(GUID);
            var nameOffset = builder.CreateString(Name);
            var infoOffset = Info.ToDisk(builder);

            Span<int> allOperateOffsetArray = stackalloc int[AllCanUseOperate.Count];
            int index = 0;
            foreach (var operate in AllCanUseOperate.Values)
                allOperateOffsetArray[index++] = operate.ToDisk(builder).Value;
            var allOperateOffset = builder.CreateVector_Offset(allOperateOffsetArray);

            Flat.RoleData.StartRoleData(builder);
            Flat.RoleData.AddTransformData(builder, transform.ToDisk(builder));
            Flat.RoleData.AddRigidbody2DData(builder, Rig2D.ToDisk(builder));
            Flat.RoleData.AddSkillPlayAgentData(builder, skillPlayAgentOffset);
            Flat.RoleData.AddNavType(builder, navTypeOffset);
            Flat.RoleData.AddToward(builder, (int)Toward);
            Flat.RoleData.AddRoleType(builder, roleTypeOffset);
            Flat.RoleData.AddGuid(builder, guidOffset);
            Flat.RoleData.AddName(builder, nameOffset);
            Flat.RoleData.AddInfo(builder, infoOffset);
            Flat.RoleData.AddAllOperate(builder, allOperateOffset);
            return Flat.RoleData.EndRoleData(builder);

        }

        public void ToRAM(Flat.RoleData diskData)
        {
            transform.ToRAM(diskData.TransformData.Value);
            Rig2D.ToRAM(diskData.Rigidbody2DData.Value);
            SkillPlayAgent.ToRAM(diskData.SkillPlayAgentData.Value);
            nav = NavManager.Inst.GetNav(diskData.NavType);
            Toward = (Toward)diskData.Toward;
            Name = diskData.Name;
            Info.ToRAM(diskData.Info.Value);
            for (int i = diskData.AllOperateLength - 1; i >= 0; i--)
            {
                var operate = OperateFSMBase.CreateOperate(diskData.AllOperate(i).Value);
                operate.Agent = this;
                AllCanUseOperate.Add(operate.GUID, operate);
            }
        }

        #endregion
    }
}
