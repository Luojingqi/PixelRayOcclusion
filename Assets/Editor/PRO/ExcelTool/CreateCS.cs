using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace ExcelTool
{
    internal static class CreateCS
    {
        public static void Run(string path, string className, string NameSpace, Dictionary<string, string> Name_Type_Dic, Dictionary<string, string> Name_Note_Dic)
        {
            #region 生成C#脚本文件
            //准备一个代码编译器单元
            CodeCompileUnit unit = new CodeCompileUnit();
            //设置自身命名空间
            CodeNamespace myNamespace = new CodeNamespace(NameSpace);
            //类名
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(className);
            //指定为类
            myClass.IsClass = true;
            //设置访问类型
            myClass.TypeAttributes = TypeAttributes.Public;
            //把这个类放在这个命名空间下
            myNamespace.Types.Add(myClass);
            //把该命名空间加入到编译器单元的命名空间集合中
            unit.Namespaces.Add(myNamespace);


            foreach (var Name_Type in Name_Type_Dic)
            {
                //字段名小写,如果原本就是小写，全面加_符号
                string NameToLower = Name_Type.Key.ToLower();
                if (NameToLower == Name_Type.Key)
                {
                    NameToLower = "_" + NameToLower;
                }
                string type = TypeNameStandardizing.Run(Name_Type.Value);
                //添加字段
                CodeMemberField field = new CodeMemberField(type, NameToLower);
                //设置访问类型
                field.Attributes = MemberAttributes.Private;
                //添加到myClass类中
                myClass.Members.Add(field);

                //添加属性
                CodeMemberProperty property = new CodeMemberProperty();
                //设置访问类型
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                //对象名称
                property.Name = Name_Type.Key;
                //有get
                property.HasGet = true;
                //有set
                property.HasSet = true;
                //设置property的类型            
                property.Type = new CodeTypeReference(type);
                //添加注释
                string note = null;
                if (Name_Note_Dic.TryGetValue(Name_Type.Key, out note))
                {
                    property.Comments.Add(new CodeCommentStatement($"<summary>\n{note}\n</summary>", true));
                }
                //get
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), NameToLower)));
                //set
                property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), NameToLower), new CodePropertySetValueReferenceExpression()));
                //添加到Customerclass类中
                myClass.Members.Add(property);

            }
            //生成C#脚本("VisualBasic"：VB脚本)
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            //代码风格:大括号的样式{}
            options.BracingStyle = "C";
            //是否在字段、属性、方法之间添加空白行
            options.BlankLinesBetweenMembers = true;

            //输出文件路径
            string outputFile = path + "\\" + className + ".cs";
            //保存
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                //为指定的代码文档对象模型(CodeDOM) 编译单元生成代码并将其发送到指定的文本编写器，使用指定的选项。(官方解释)
                //将自定义代码编译器(代码内容)、和代码格式写入到sw中
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }
            #endregion
        }
    }
}
