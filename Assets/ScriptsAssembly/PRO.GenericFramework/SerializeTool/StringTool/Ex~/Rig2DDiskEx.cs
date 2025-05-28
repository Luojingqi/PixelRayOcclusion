using System.Text;
using UnityEngine;

namespace PRO.Tool.Serialize.String
{
    public static class Rig2DDiskEx
    {
        public static StringBuilder ToDisk(this Rigidbody2D rig)
        {
            var sb = SetPool.TakeOut_SB();
            sb.AddStart(nameof(Rigidbody2D));

            sb.Add(rig.velocity);
            sb.Append(',');
            sb.Add(rig.angularVelocity);
            sb.Append(',');
            sb.Add(rig.simulated);

            sb.AddEnd();
            return sb;
        }

        public static int ToRAM(this Rigidbody2D rig, string text, int offset)
        {
            return StringTool.Deserialize_平行(text, (list) =>
            {
                int index = 0;
                var v = new Vector2(list[index++].ToFloat(), list[index++].ToFloat());
                rig.velocity = v;// new Vector2(list[index++].ToFloat(), list[index++].ToFloat());
                rig.angularVelocity = list[index++].ToFloat();
                rig.simulated = list[index++].ToBool();
                Debug.Log(v.x + "|" + v.y);
                Debug.Log(rig.velocity.x + "|" + rig.velocity.y);
            },
            offset);
        }
    }
}