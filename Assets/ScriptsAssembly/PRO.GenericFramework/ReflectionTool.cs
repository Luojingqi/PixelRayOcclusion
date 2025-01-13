using System.Collections.Generic;
using System;
using System.Reflection;

namespace PROTool
{
    public static class ReflectionTool
    {
        /// <summary>
        /// 获取所有继承了该类型的类型，包含子类的子类
        /// </summary>
        public static List<Type> GetDerivedClasses(this Type type)
        {
            List<Type> derivedClasses = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                    {
                        derivedClasses.Add(t);
                    }
                }
            }
            return derivedClasses;
        }
    }
}
