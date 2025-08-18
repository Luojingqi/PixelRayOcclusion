using NodeCanvas.Framework;
using UnityEngine;
namespace PRO.BT.Tool
{
    public class 距离判断_Vector2 : ConditionTask
    {
        public BBParameter<Vector2> Pos0;
        public BBParameter<Vector2> Pos1;
        public BBParameter<float> Distance;

        protected override bool OnCheck()
        {
            return Vector2.Distance(Pos0.value, Pos1.value) <= Distance.value;
        }
    }

    public class 距离判断_Vector2Int : ConditionTask
    {
        public BBParameter<Vector2Int> Pos0;
        public BBParameter<Vector2Int> Pos1;
        public BBParameter<float> Distance;

        protected override bool OnCheck()
        {
            return Vector2Int.Distance(Pos0.value, Pos1.value) <= Distance.value;
        }
    }
}
