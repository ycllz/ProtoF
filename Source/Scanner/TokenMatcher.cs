﻿

namespace ProtoTool.Scanner
{
    public abstract class TokenMatcher
    {
        bool _ignore;

        public abstract Token Match(Lexer tz);
        public TokenMatcher Ignore( )
        {
            _ignore = true;
            return this;
        }

        public bool IsIgnored
        {
            get { return _ignore; }
        }
    }
}
