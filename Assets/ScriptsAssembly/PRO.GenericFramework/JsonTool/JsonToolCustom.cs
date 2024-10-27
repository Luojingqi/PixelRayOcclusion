using UnityEngine;
using LitJson;
namespace PRO.Tool
{
    /// <summary>
    /// LitJson自定义扩展
    /// </summary>
    public static class JsonToolCustom
    {
        public static bool isInit = false;
        public static void Init()
        {
            TypeConversionRegister();
            WriteRegister();
            ReadRegister();
            isInit = true;
        }
        /// <summary>
        /// 导出寄存器
        /// </summary>
        public static void WriteRegister()
        {
            JsonMapper.RegisterExporter<Vector4>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteProperty("w", v.w);
                w.WriteObjectEnd();
            });
            JsonMapper.RegisterExporter<Vector3>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteProperty("z", v.z);
                w.WriteObjectEnd();
            });
            JsonMapper.RegisterExporter<Vector2>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            });
            JsonMapper.RegisterExporter<Vector2Int>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("x", v.x);
                w.WriteProperty("y", v.y);
                w.WriteObjectEnd();
            });


            //Color
            JsonMapper.RegisterExporter<Color>((v, w) =>
            {
                w.WriteObjectStart();
                w.WriteProperty("r", v.r);
                w.WriteProperty("g", v.g);
                w.WriteProperty("b", v.b);
                w.WriteProperty("a", v.a);
                w.WriteObjectEnd();
            });


            //JsonMapper.RegisterExporter<Fix64>((v, w) =>
            //{
            //    w.Write(v.value);
            //});
        }

        /// <summary>
        /// 导入寄存器
        /// </summary>
        public static void ReadRegister()
        {
            //JsonMapper.RegisterObjectConvert(new PropInfosRegister());
        }
        //public class PropInfosRegister : IJsonConvert<PropInfos.Info>
        //{
        //    public PropInfos.Info Convert(IJsonWrapper input)
        //    {
        //        int index = (int)((IJsonWrapper)input["propType"]).GetLong();

        //        switch ((PropInfos.PropType)index)
        //        {
        //            case PropInfos.PropType.Command: return input.ToObject<PropInfos.CommandInfo>();
        //            case PropInfos.PropType.BuildCube: return input.ToObject<PropInfos.BuildCubeInfo>();
        //            case PropInfos.PropType.BuildPrefab: return input.ToObject<PropInfos.BuildPrefabInfo>();
        //        }
        //        return null;
        //    }
        //}

        /// <summary>
        /// 类型转换寄存器
        /// </summary>
        public static void TypeConversionRegister()
        {
            JsonMapper.RegisterImporter<long, int>((input) =>
            {
                return (int)input;
            });
            JsonMapper.RegisterImporter<long, byte>((input) =>
            {
                return (byte)input;
            });
            JsonMapper.RegisterImporter<long, float>((input) =>
            {
                return (float)input;
            });
        }


    }
}