namespace PRO.Weapon
{   
    public partial class Weapon_0_0
    {
        protected override void SetAutoConfigValue(PRO.Weapon.WeaponConfig.Data data)
        {
            this.Auto_挥砍距离 = (data.ValueDic["Auto_挥砍距离"] as ConfigValue<System.Int32>).Value;

        }
    }
}