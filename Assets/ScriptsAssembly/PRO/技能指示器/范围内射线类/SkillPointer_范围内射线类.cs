using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace PRO.Skill
{
    public class SkillPointer_范围内射线类 : SkillPointer_范围内选择类
    {
        [OdinSerialize]
        [ShowInInspector]
        public float Density
        {
            get { return density; }
            set
            {
                density = value;
                UpdateTiling();
            }
        }
        private float density = 1;

        [ShowInInspector]
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        private float speed;

        public LineRenderer Line { get; private set; }


        [ShowInInspector]
        public Vector2 StartPos
        {
            get { return startPos; }
            set
            {
                startPos = value;
                Line.SetPosition(0, value);
                UpdateTiling();
            }
        }
        private Vector2 startPos;

        [ShowInInspector]
        public Vector2 EndPos
        {
            get { return endPos; }
            set
            {
                endPos = value;
                Line.SetPosition(1, value);
                UpdateTiling();
            }
        }
        private Vector2 endPos;

        public void UpdateTiling()
        {
            if (Line == null) return;
            Line.textureScale = new Vector2((endPos - startPos).magnitude * density, 1);
        }
        public void UpdateOffset()
        {
            if (Line == null) return;
            Line.material.mainTextureOffset += new Vector2(speed, 0);
        }

        public override void Init()
        {
            base.Init();
            Line = transform.GetComponent<LineRenderer>();
            Density = density;
        }
    }
}
