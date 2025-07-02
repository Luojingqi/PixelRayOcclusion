namespace PRO.Skill
{
    public class SkillPointer_范围内选择类 : SkillPointerBase
    {
        public void SetPointer(int Radius_G)
        {
            RangeBox.sprite = RoleManager.Inst.SkillPointerAsset.RangeBoxSpriteArray[Radius_G];
        }
    }
}
