using UnityEngine;

namespace PRO.Skill
{
    public class SkillPointer_箭头类 : SkillPointerBase
    {
        /// <summary>
        /// 箭头
        /// </summary>
        public SpriteRenderer Arrow { get; private set; }
        public override void Init()
        {
            base.Init();
            Arrow = Center.Find("Arrow").GetComponent<SpriteRenderer>();
        }

        public override void Close()
        {
            base.Close();
            transform.position = Vector3.zero;
            Center.rotation = Quaternion.identity;
        }
        public void UpdatePointer()
        {
            Center.rotation = Quaternion.FromToRotation(Vector3.up, MousePoint.mousePos);
        }
    }
}
