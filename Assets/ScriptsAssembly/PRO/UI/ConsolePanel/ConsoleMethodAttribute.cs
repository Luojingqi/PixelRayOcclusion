using System;

namespace PRO.Console
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class ConsoleMethodAttribute : Attribute
    {
        public string MethodHeader { get; set; }
        public ConsoleMethodAttribute(string MethodHeader)
        {
            this.MethodHeader = MethodHeader;
        }
    }
}
