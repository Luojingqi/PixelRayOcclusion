﻿using Google.FlatBuffers;
using PRO.Skill;
using PRO.Tool;
using UnityEngine;

namespace PRO.TurnBased
{
    /// <summary>
    /// T1阶段：技能目标选择
    /// </summary>
    public abstract class OperateStateBase_T1 : IFSMState<OperateStateEnum>
    {
        public abstract OperateFSMBase Operate { get; set; }
        public FSMManager<OperateStateEnum> FSM { get => Operate; set => Operate = (OperateFSMBase)value; }

        public OperateStateEnum EnumName => OperateStateEnum.t1;


        public virtual void Enter() { lastPixelPosRotate = PixelPosRotate.New(0, 0, 0); }
        public void Exit() { Operate.Agent.gameObject.layer = (int)GameLayer.Role; DestroyPointer(); }

        public void Update()
        {
            if (Operate.form != OperateFSMBase.Operator.Player) return;
            FlatBufferBuilder record = FlatBufferBuilder.TakeOut(1024 * 4);
            OperateFSMBase.TriggerState state = Trigger(record);

            if (state == OperateFSMBase.TriggerState.toT2)
            {
                //MCTS.Node node = MCTS.Node.TakeOut();
                //node.operateRecord = operateRecord;
                节点执行(record , OperateFSMBase.Operator.Player);
                Operate.Agent.ForEachBuffApplyEffect(BuffTriggerType.技能释放前, Operate.context, -1);
                Operate.SwitchState(OperateStateEnum.t2);
                Operate.T2.Enter(record);
                //foreach (var item in Operate.Agent.AllCanUseOperate)
                //    if (item.GridUI != null) item.UpdateUI();

                //MCTS.Node.PutIn(node);
            }
            else
            {
                if (state == OperateFSMBase.TriggerState.toT0)
                {
                    Operate.Agent.Toward = Operate.startToward;
                    CombatContext.PutIn(Operate.context);
                    Operate.context = null;
                    Operate.SwitchState(OperateStateEnum.t0);
                }
                else
                {
                    Operate.lastToward = Operate.Agent.Toward;
                }
            }
            FlatBufferBuilder.PutIn(record);
        }

        /// <summary>
        /// 持续触发，一般用于技能指示器的更新
        /// </summary>
        public abstract OperateFSMBase.TriggerState Trigger(FlatBufferBuilder record);

        /// <summary>
        /// 销毁技能指示器
        /// </summary>
        public virtual void DestroyPointer()
        {
            if (Operate is ISkillPointer i)
            {
                i.SkillPointerBase?.Close();
                i.SkillPointerBase = null;
            }
        }


        protected PixelPosRotate lastPixelPosRotate = PixelPosRotate.New(0, 0, 0);
        private float time;
        /// <summary>
        /// 按键更新旋转
        /// </summary>
        /// <param name="activeX"></param>
        /// <param name="activeY"></param>
        /// <param name="activeZ"></param>
        /// <returns></returns>
        public PixelPosRotate UpdatePixelPosRotate(bool activeX = true, bool activeY = true, bool activeZ = true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse3)) time = Time.time;
            if (Input.GetKeyUp(KeyCode.Mouse3) && Time.time - time < 0.25f)
                lastPixelPosRotate = PixelPosRotate.New(lastPixelPosRotate.z90Num, lastPixelPosRotate.y180Num + 1, lastPixelPosRotate.x180Num);

            if (Input.GetKey(KeyCode.Mouse3))
            {
                int z90Num = lastPixelPosRotate.z90Num;
                int y180Num = lastPixelPosRotate.y180Num;
                int x180Num = lastPixelPosRotate.x180Num;
                float axis = Input.GetAxis("Mouse ScrollWheel");
                if (axis > 0) z90Num += y180Num == 1 ? -1 : 1;
                else if (axis < 0) z90Num += y180Num == 1 ? 1 : -1;
                var posRotate = PixelPosRotate.New(z90Num, y180Num, x180Num);
                posRotate.activeX = activeX;
                posRotate.activeY = activeY;
                posRotate.activeZ = activeZ;
                lastPixelPosRotate = posRotate;
                return posRotate;
            }
            else return lastPixelPosRotate;
        }

        public void RotateToMouse(Transform transform, Vector2 center, Vector2 offset)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, MousePoint.worldPos - center);
            transform.position = (Vector3)center - transform.rotation * offset;
        }
        public void RotateToMouse(Transform transform, Vector2Int center, Vector2Int offset) => RotateToMouse(transform, Block.GlobalToWorld(center), Block.GlobalToWorld(offset));

        public abstract void 节点扩展(ref ReusableList<FlatBufferBuilder> recordList);

        public abstract void 节点执行(FlatBufferBuilder record, OperateFSMBase.Operator form);
    }
}
