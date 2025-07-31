using System.Text;
using UnityEngine;

namespace PRO.Proto
{
    public static class TransformDiskEx
    {

        //public static StringBuilder ToDisk(this Transform transform)
        //{
        //    var sb = SetPool.TakeOut_SB();
        //    sb.AddStart(nameof(Transform));

        //    sb.Add(transform.position);
        //    sb.Append(',');
        //    sb.Add(transform.rotation);
        //    sb.Append(',');
        //    sb.Add(transform.localScale);

        //    sb.AddEnd();
        //    return sb;
        //}

        //public static int ToRAM(this Transform transform, string text, int offset)
        //{
        //    return StringTool.Deserialize_平行(text, (list) =>
        //    {
        //        int index = 0;
        //        transform.position = new Vector3(list[index++].ToFloat(), list[index++].ToFloat(), list[index++].ToFloat());
        //        transform.rotation = new Quaternion(list[index++].ToFloat(), list[index++].ToFloat(), list[index++].ToFloat(), list[index++].ToFloat());
        //        transform.localScale = new Vector3(list[index++].ToFloat(), list[index++].ToFloat(), list[index++].ToFloat());
        //    },
        //    offset);
        //}
    }
}