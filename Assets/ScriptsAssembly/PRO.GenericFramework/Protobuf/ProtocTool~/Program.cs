using System;
using System.IO;
using System.Text;

namespace PRO.Proto.ProtocTool
{
    internal class Program
    {
        public static StringBuilder cs = new StringBuilder();
        static void Main(string[] commands)
        {
            //commands = new string[] {
            //@"E:\Projects\Unity_P\PixelRayOcclusion\Assets\ScriptsAssembly\PRO\Block\BlockBaseData.proto",
            // @"E:/Projects/Unity_P/PixelRayOcclusion/Assets/ScriptsAssembly/PRO.GenericFramework/Protobuf/CSharp/"};
            try
            {

                var lines = File.ReadAllLines(commands[0]);
                for (int i = 0; i < lines.Length; i++)
                {
                    var strs = Message.Trim(lines[i].Split(' ', ';'));
                    switch (strs[0])
                    {
                        case "package":
                            cs.AppendLine($"namespace {strs[1]}");
                            cs.AppendLine("{");
                            break;
                        case "message":
                            cs.AppendLine(Message.Start(lines, strs[1], ref i).ToString());
                            break;

                    }
                }
                cs.AppendLine("}");
                File.AppendAllText(commands[1] + Path.GetFileNameWithoutExtension(commands[0]) + ".cs", cs.ToString());
                Console.WriteLine();
                Console.WriteLine("生成扩展脚本成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("生成扩展脚本失败");
                Console.WriteLine(commands[0]);
                Console.WriteLine(commands[1]);
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
