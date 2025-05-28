using Humanizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace PRO.Proto.ProtocTool
{
    internal class Message
    {
        public static HashSet<string> EnumSet = new HashSet<string>();

        public List<Message> Types = new List<Message>();
        public List<Value> values = new List<Value>();
        public List<OneOf> oneOfs = new List<OneOf>();

        public string name;
        public static Message Start(string[] lines, string typeName, ref int i)
        {
            Message msg = new Message();
            msg.name = typeName;
            i += 2;
            while (true)
            {
                var line = lines[i];

                var strs = Trim(lines[i].Split( ' ', ';' ));
                switch (strs[0])
                {
                    case "message":
                        msg.Types.Add(Message.Start(lines, $"{msg.name}.Types.{strs[1]}", ref i));
                        break;
                    case "}": return msg;
                    case "oneof":
                        //msg.oneOfs.Add(Message.StartOneOf(lines, strs[1], ref i));
                        msg.values.AddRange(Message.StartOneOf(lines, strs[1], ref i).value);
                        break;
                    case "enum":
                        EnumSet.Add($"{strs[1]}");
                        // Console.WriteLine($"{msg.name}.{strs[1]}");
                        StartEnum(lines, ref i);
                        break;
                    default:
                        if (strs[0].Length == 0 || strs[0][0] == '/') break;
                        msg.values.Add(Value.Start(strs));
                        break;
                }
                i++;
            }
        }
        public class OneOf
        {
            public List<Value> value = new List<Value>();
        }

        public class Value
        {
            public string type;
            public string name;

            public static Value Start(string[] strs)
            {
                if (strs[0] == "repeated")
                {
                    return new ValueList() { type = strs[1], name = strs[2].Pascalize() };
                }
                else
                {
                    if (strs[0].Substring(0, 3) == "map")
                    {
                        var dic = Trim(strs[0].Split('<', '>', ',', ' '));
                        return new ValueMap() { type = dic[0], valueType = dic[1], name = strs[1].Pascalize() };
                    }
                    else
                    {
                        return new Value() { type = strs[0], name = strs[1].Pascalize() };
                    }
                }
            }

        }
        public class ValueList : Value { }
        public class ValueMap : Value { public string valueType; }

        public static OneOf StartOneOf(string[] lines, string oneOfName, ref int i)
        {
            var oneof = new OneOf();
            i += 2;
            while (true)
            {
                var line = lines[i];
                var strs = Trim(lines[i].Split(' ', ';'));
                switch (strs[0])
                {
                    case "}": return oneof;
                    default:
                        if (strs[0].Length == 0 || strs[0][0] == '/') break;
                        oneof.value.Add(Value.Start(strs));
                        break;
                }
                i++;
            }
        }
        public static void StartEnum(string[] lines, ref int i)
        {
            i += 2;
            while (true)
            {
                var line = lines[i];
                var strs = Trim(lines[i].Split(' ', ';'));
                switch (strs[0])
                {
                    case "}": return;
                }
                i++;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var names = name.Split('.');
            sb.AppendLine($"public partial class {names[names.Length - 1]}");
            sb.AppendLine("{");
            sb.AppendLine("public void Clear()");
            sb.AppendLine("{");
            foreach (var value in values)
            {
                switch (value)
                {
                    case ValueList v:
                        {
                            if (BasicTypeDic.ContainsKey(v.type))
                            {
                                sb.AppendLine($"{v.name}.Clear();");
                            }
                            else if (EnumSet.Contains(v.type))
                            {
                                sb.AppendLine($"{v.name}.Clear();");
                            }
                            else
                            {
                                sb.AppendLine($"foreach(var item in {v.name})");
                                sb.AppendLine("{");
                                if (v.type == "bytes")
                                {
                                    sb.AppendLine($"ByteString.PutIn(item);");
                                }
                                else
                                {
                                    sb.AppendLine($"item.Clear();");
                                    sb.AppendLine($"item.PutIn();");
                                }
                                sb.AppendLine("}");
                                sb.AppendLine($"{v.name}.Clear();");
                            }
                            break;
                        }
                    case ValueMap v:
                        {
                            if (BasicTypeDic.ContainsKey(v.valueType))
                            {
                                sb.AppendLine($"{v.name}.Clear();");
                            }
                            else if (EnumSet.Contains(v.valueType))
                            {
                                sb.AppendLine($"{v.name}.Clear();");
                            }
                            else
                            {
                                sb.AppendLine($"foreach(var kv in {value.name})");
                                sb.AppendLine("{");
                                if (v.valueType == "bytes")
                                {
                                    sb.AppendLine($"ByteString.PutIn(kv.Value);");
                                }
                                else
                                {
                                    sb.AppendLine($"kv.Value.Clear();");
                                    sb.AppendLine($"kv.Value.PutIn();");
                                }
                                sb.AppendLine("}");
                                sb.AppendLine($"{v.name}.Clear();");
                            }
                            break;
                        }
                    default:
                        {
                            if (value.type == "bytes")
                            {
                                sb.AppendLine($"if({value.name}!=null)");
                                sb.AppendLine("{");
                                sb.AppendLine($"ByteString.PutIn({value.name});");
                                sb.AppendLine($"{value.name} = null;");
                                sb.AppendLine("}");
                            }
                            else if (EnumSet.Contains(value.type))
                            {
                                sb.AppendLine($"{value.name} = (Types.{value.type})0;");
                            }
                            else if (BasicTypeDic.ContainsKey(value.type))
                            {
                                sb.AppendLine($"{value.name} = {BasicTypeDic[value.type]};");
                            }
                            else
                            {
                                sb.AppendLine($"if({value.name}!=null)");
                                sb.AppendLine("{");
                                sb.AppendLine($"{value.name}.Clear();");
                                sb.AppendLine($"{value.name}.PutIn();");
                                sb.AppendLine($"{value.name} = null;");
                                sb.AppendLine("}");
                            }
                            break;
                        }
                }

            }
            sb.AppendLine("}");
            if (Types.Count > 0)
            {
                sb.AppendLine($"public partial class Types");
                sb.AppendLine("{");
                foreach (var type in Types)
                    sb.Append(type.ToString());
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static Dictionary<string, string> BasicTypeDic = new Dictionary<string, string>()
        {
            {"double" , "0"},
            {"float" , "0"},
            {"int32" , "0"},
            {"int64" , "0"},
            {"uint32" , "0"},
            {"uint64" , "0"},
            {"sint32" , "0"},
            {"sint64" , "0"},
            {"fixed32" , "0"},
            {"fixed64" , "0"},
            {"sfixed32" , "0"},
            {"sfixed64" , "0"},
            {"bool" , "false"},
            {"string" , "null"},
        };

        public static string[] Trim(string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
                strs[i] = strs[i].Trim();
            return strs;
        }
    }

}
