using NodeCanvas.Framework;
using PRO.SkillEditor;


namespace PRO.BT.攻击
{
    public class 依据角色攻击速度设置动画播放速度 : ActionTask
    {
        public BBParameter<Role> Agent;
        public BBParameter<SkillPlayData> PlayData;
        protected override void OnExecute()
        {
            PlayData.value.scale = (float)Agent.value.Info.攻击速度.ValueSum;
            EndAction(true);
        }
    }
}