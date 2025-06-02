using System.Text;
using UnityEngine;

namespace PRO.Tool.Serialize.String
{
    public static class RaycastHit2DEx
    {
        //public static StringBuilder ToDisk(this RaycastHit2D hit)
        //{
        //    var sb = SetPool.TakeOut_SB();
        //    sb.AddStart(nameof(Rigidbody2D));

        //    sb.Add(hit.centroid);
        //    sb.Append(',');
        //    sb.Add(hit.distance);
        //    sb.Append(',');
        //    sb.Add(hit.fraction);
        //    sb.Append(',');
        //    sb.Add(hit.normal);
        //    sb.Append(',');
        //    sb.Add(hit.point);

        //    sb.AddEnd();
        //    return sb;
        //}
        //public static int ToRAM(this RaycastHit2D hit, string text, int offset)
        //{
        //    return StringTool.Deserialize_平行(text, (list) =>
        //    {
        //        int index = 0;
        //        hit.centroid = new(list[index++].ToFloat(), list[index++].ToFloat());
        //        hit.distance = list[index++].ToFloat();
        //        hit.normal = new(list[index++].ToFloat(), list[index++].ToFloat());
        //        hit.point = new(list[index++].ToFloat(), list[index++].ToFloat());
        //    },
        //    offset);
        //}
    }
}
