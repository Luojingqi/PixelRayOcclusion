using Cysharp.Threading.Tasks;
using PRO.Buff;
using PRO.Buff.Base;
using PRO.Buff.Base.IBuff;
using PRO.Proto.Ex;
using PRO.SkillEditor;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace PRO
{
    public class Role : MonoScriptBase, IScene, ITime_Start, ITime_Update
    {
        public SpriteRenderer SpriteRenderer { get; private set; }
        public SkillPlayAgent SkillPlayAgent { get; private set; }

        public Rigidbody2D Rig2D { get; private set; }
        public Dictionary<string, OperateFSMBase> AllCanUseOperate { get; } = new Dictionary<string, OperateFSMBase>();

        [NonSerialized]
        public FreelyLightSource source;
        [NonSerialized]
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
        public string Guid;
        public string Name;
        public Sprite Icon;
        [NonSerialized]
        public RoleInfo Info = new RoleInfo();
        public void TimeStart()
        {
            _scene = SceneManager.Inst.NowScene;

            for (int i = 0; i < AllBuff.Length; i++)
                AllBuff[i] = new List<IBuffBase>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SkillPlayAgent = GetComponent<SkillPlayAgent>();
            SkillPlayAgent.Init();
            SkillPlayAgent.SetScene(_scene);

            Rig2D = GetComponent<Rigidbody2D>();

            nav = NavManager.GetNav("默认");

            Guid = System.Guid.NewGuid().ToString();

            //CanUseOperateList.Add(new Skill_1_0());
            //CanUseOperateList.Add(new Skill_1_1());
            //CanUseOperateList.Add(new Skill_3_0());
            //AllCanUseOperate.Add(new Skill_3_3());
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
            AddBuff(new Buff_1_0());
            AddBuff(new Buff_2_0());
            AddBuff(new Buff_2_1());
            AddBuff(new Buff_2_3());
            AddBuff(new Buff_2_5());
            AddBuff(new Buff_2_7());
            //AddBuff(new Buff_2_8());
            AddBuff(new Buff_2_9());
            AddBuff(new Buff_2_10());
            AddBuff(new Buff_2_11());
            source = FreelyLightSource.New(_scene, new Color32(200, 200, 200, 255), 15);


            PROMain.Inst.AddRole(this);
        }
        public void TimeUpdate()
        {
            source.GloabPos = Block.WorldToGlobal(transform.position) + nav.AgentMould.center;
            for (int i = 0; i < AllBuff.Length; i++)
            {
                List<IBuffBase> buffList = AllBuff[i];
                for (int j = 0; j < buffList.Count; j++)
                    buffList[j].UpdateCheckAction();
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

        #region buff
        public List<IBuffBase>[] AllBuff { get; } = new List<IBuffBase>[(int)BuffTriggerType.end];

        public Dictionary<string, IBuffBase> BuffNameDic = new Dictionary<string, IBuffBase>();
        public Dictionary<Type, IBuffBase> BuffTypeDic = new Dictionary<Type, IBuffBase>();
        public void AddBuff(IBuffBase buff)
        {
            AllBuff[(int)buff.TriggerType].Add(buff);
            BuffNameDic.Add(buff.Name, buff);
            if (buff is IBuff_独有) BuffTypeDic.Add(buff.GetType(), buff);
            buff.RoleAddThis(this);
        }
        public void RemoveBuff(IBuffBase buff)
        {
            AllBuff[(int)buff.TriggerType].Remove(buff);
            BuffNameDic.Remove(buff.Name);
            if (buff is IBuff_独有) BuffTypeDic.Remove(buff.GetType());
            buff.RoleRemoveThis();
        }
        public void UpdateBuffUI()
        {
            if (GameMainUIC.Inst.NowShowRole != this) return;
            GameMainUIC.Inst.UpdateNowShowRoleBuff();
        }
        public IBuffBase GetBuff(string buffName)
        {
            BuffNameDic.TryGetValue(buffName, out IBuffBase buff);
            return buff;
        }
        public T GetBuff<T>() where T : class, IBuffBase
        {
            BuffTypeDic.TryGetValue(typeof(T), out IBuffBase buff);
            return buff as T;
        }

        public void ForEachBuffApplyEffect(BuffTriggerType type, CombatContext context, int byAgentIndex)
        {
            List<IBuffBase> buffList = AllBuff[(int)type];
            for (int i = buffList.Count - 1; i >= 0; i--)
            {
                var buff = buffList[i];
                if (buff.GetActive()) buff.ApplyEffect(context, byAgentIndex);
            }
        }
        #endregion

        #region 序列化与反序列化

        public Proto.RoleData ToDisk()
        {
            var diskData = Proto.ProtoPool.TakeOut<Proto.RoleData>();
            diskData.TransFormData = transform.ToDisk();
            diskData.Rigidbody2DData = Rig2D.ToDisk();
            diskData.SkillPlayAgentData = SkillPlayAgent.ToDisk();
            diskData.NavType = nav.TypeName;
            diskData.Toward = (int)Toward;
            diskData.RoleTypeName = RoleTypeName;
            diskData.GUID = Guid;
            diskData.Name = Name;
            diskData.Info = Info.ToDisk();


            return diskData;
        }

        public void ToRAM(Proto.RoleData diskData)
        {
            transform.ToRAM(diskData.TransFormData);
            Rig2D.ToRAM(diskData.Rigidbody2DData);
            SkillPlayAgent.ToRAM(diskData.SkillPlayAgentData);
            nav = NavManager.GetNav(diskData.NavType);
            Toward = (Toward)diskData.Toward;
            Guid = diskData.GUID;
            Name = diskData.Name;
            Info.ToRAM(diskData.Info);

        }

        #endregion
    }
}
