﻿using ProtoF.AST;
using ProtoF.Printer;
using System.Text;

namespace ProtoF.Printer
{
    partial class ProtoFPrinter : IPrinter
    {
        public void Print(FileNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("package {0}\n", node.Package);

            foreach (var n in node.Child)
            {
                n.PrintVisit(this, sb, opt, values);                
            }
        }

        public void Print(CommentNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.AppendFormat("{0}//{1}\n", opt.MakeIndentSpace(), node.Comment);
        }

        public void Print(EOLNode node, StringBuilder sb, PrintOption opt, params object[] values)
        {
            sb.Append("\n");
        }
    }
}
