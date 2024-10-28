namespace ExcelTool
{
    internal static class TypeNameStandardizing
    {
        //命名空间
        public static string _System = "System.";
        public static string _UnityEngine = "UnityEngine.";
        public static string _SystemCollectionsGeneric = "System.Collections.Generic.List";
        public static string _Fix64PhysicsData = "Fix64Physics.Data.";
        public static string _UnityMathematics = "Unity.Mathematics.";

        /// <summary>
        /// 从Excel中读取的类型字符串转换为带命名空间的标准名
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static string Run(string typeString)
        {
            bool IsList = typeString.Contains("[]");
            string typeName = null;
            if (IsList) typeString = typeString.Split('[')[0];
            switch (typeString.ToLower())
            {
                case "string": typeName = _System + "String"; break;
                case "int": typeName = _System + "Int32"; break;
                case "bool": typeName = _System + "Boolean"; break;
                case "double": typeName = _System + "Double"; break;
                case "float": typeName = _System + "Single"; break;
                case "vector3": typeName = _UnityEngine + "Vector3"; break;
                case "vector4": typeName = _UnityEngine + "Vector4"; break;
                case "int4": typeName = _UnityMathematics + "int4"; break;
                case "uint4": typeName = _UnityMathematics + "uint4"; break;
                case "color32": typeName = _UnityEngine + "Color32"; break;
                case "fix64": typeName = _Fix64PhysicsData + "Fix64"; break;
                case "fixvector3": typeName = _Fix64PhysicsData + "FixVector3"; break;
                case "fixvector2": typeName = _Fix64PhysicsData + "FixVector2"; break;
                case "fixquaternion": typeName = _Fix64PhysicsData + "FixQuaternion"; break;
                default: typeName = "未定义"; break;
            }
            if (IsList)
            {
                typeName = _SystemCollectionsGeneric + "<" + typeName + ">";
            }
            return typeName;
        }
    }
}
