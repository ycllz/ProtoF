﻿using ProtoTool.Schema;
using System.Text;
using System.Linq;
using System;

namespace ProtoTool.Schema
{
    public enum PrintFormat
    {
        ProtoF,
        Protobuf,
    }


    public struct PrintOption
    {
        public PrintFormat Format;
        public int Indent;
        public bool ShowAllFieldNumber; // 显示所有字段序号
        public bool ShowAllEnumNumber; // 显示所有枚举序号
        public bool AutoGenHeader; // 显示文件头

        public PrintOption(PrintOption parent)
        {
            this = (PrintOption)parent.MemberwiseClone();

            Indent = parent.Indent + 1;
        }

        // 按缩进生成tab
        public string MakeIndentSpace()
        {
            if (Indent == 0)
                return string.Empty;

            return "".PadRight(Indent,'\t');
        }
    }


    public class Printer
    {
        public virtual void Print(FileNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            if ( opt.AutoGenHeader )
            {
                sb.AppendLine("// Generated by ProtoTool, DO NOT MODIFIED!");
            }

            foreach (var n in node.Child)
            {
                n.PrintVisit(this, sb, opt, values);
            }
        }

        public virtual void Print(CommentNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("{0}//{1}\n", opt.MakeIndentSpace(), node.Comment);
        }

        public virtual void Print(EOLNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.Append("\n");
        }

        public virtual void Print(PackageNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("package {0}\n", node.Name);
        }

        public virtual void Print(ImportNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("import \"{0}\"\n", node.Name);
        }


        public virtual void Print(MessageNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            throw new NotImplementedException();
        }

        public virtual void Print(FieldNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            var maxNameLength = (int)values[0];
            var maxTypeLength = (int)values[1];

            sb.Append(opt.MakeIndentSpace());

            // 类型
            {
                sb.Append(node.CompleteTypeName.PadRight(maxTypeLength + 1));
            }

            // 字段名
            {
                sb.Append(node.Name.PadRight(maxNameLength + 1));                
            }


            // 序号
            {
                if (node.Number > 0 && (!node.NumberIsAutoGen || opt.ShowAllFieldNumber))
                {
                    sb.AppendFormat(" = {0} ", node.Number);
                }
            }


            // Option
            if (node.HasOption)
            {

                sb.Append("[");

                if (node.DefaultValue != "")
                {
                    sb.AppendFormat("default:{0}", node.DefaultValue);
                }

                sb.Append("] ");
            }



            // 注释
            if (!string.IsNullOrEmpty(node.TrailingComment))
            {
                sb.AppendFormat("//{0}", node.TrailingComment);
            }

            sb.Append("\n");
        }


        public virtual void Print(EnumValueNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {

            var maxNameLength = (int)values[0];

            sb.Append(opt.MakeIndentSpace());

            sb.Append(node.Name.PadRight(maxNameLength));

            if ((!node.NumberIsAutoGen || opt.ShowAllEnumNumber))
            {

                sb.AppendFormat(" = {0}", node.Number);
            }

            var commentSpace = " ".PadLeft(Math.Max( 3 - node.Number.ToString().Length, 1));
            sb.Append(commentSpace);

            if (!string.IsNullOrEmpty(node.TrailingComment))
            {
                sb.AppendFormat("//{0}", node.TrailingComment);
            }

            sb.Append("\n");
        }

        public virtual void Print(EnumNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.Append(opt.MakeIndentSpace());

            sb.AppendFormat("enum {0}\n", node.Name);
            sb.Append(opt.MakeIndentSpace());
            sb.Append("{\n");

            int maxNameLength = 0;

            if ( node.Value.Count > 0 )
            {
                maxNameLength = node.Value.Select(x => x.Name.Length).Max();
            }
            

            var subopt = new PrintOption(opt);

            foreach (var n in node.Child)
            {
                n.PrintVisit(this, sb, subopt, maxNameLength);
            }

            sb.Append(opt.MakeIndentSpace());
            sb.Append("}\n");
        }
    }
}
