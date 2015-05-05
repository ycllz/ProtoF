﻿using ProtoF.AST;
using ProtoF.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoF.Parser
{
    public partial class SchemaParser
    {

        // 解析纯注释, 不是注释跳过
        string ParseComment(Node parent)
        {
            string comment = null;
            switch (CurrToken.Type)
            {
                case TokenType.Comment:
                    {
                        comment = RawParseComment(parent);
                        break;
                    }
                default:
                    return null;
            }

            return comment;
        }

        void ParseTrailingComment(TrailingCommentNode parent)
        {
            if (CurrToken.Type == TokenType.EOL)
            {
                TryConsume(TokenType.EOL);
            }
            else
            {
                var comment = ParseComment(parent);
                parent.TrailingComment = comment;
            }
        }

        string RawParseComment(Node parent)
        {
            var n = new CommentNode();
            MarkLocation(n);            
            n.Comment = CurrToken.Value;
            parent.Add(n);

            Next();

            FetchToken(TokenType.EOL, "comment must has EOL end");

            return n.Comment;
        }

        // 解析非上下文关联的注释及回车
        void ParseCommentAndEOL(Node parent)
        {
            while (true)
            {
                switch (CurrToken.Type)
                {
                    case TokenType.EOL:
                        {
                            var n = new EOLNode();
                            MarkLocation(n);
                            parent.Add(n);
                            Next();
                            break;
                        }
                    case TokenType.Comment:
                        {
                            RawParseComment(parent);
                            break;
                        }
                    default:
                        return;
                }

                
            }

        }

    }
}