using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class StringConstant : IComponent
    {
        private static readonly Regex _regTag = new Regex("<[^>]+>", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static readonly Regex _regSpaces = new Regex("\\s\\s+", RegexOptions.Compiled);

        private string _text;
        private bool _inPre;
        private string _processed;
        public StringConstant(string text,bool inPre)
        {
            _text = text;
            _inPre = inPre;
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable,bool compress)
        {
            if (_processed == null)
                _processed = _CleanHTML();
            return _processed;
        }

        private string _CleanHTML()
        {
            if (_inPre)
                return "ret+='" + _text.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t") + "';";
            else
            {
                StringBuilder sb = new StringBuilder();
                var index = 0;
                Match m = _regTag.Match(_text, index);
                while (m.Value != "")
                {
                    if (m.Value == "</pre>")
                        sb.Append(_text.Substring(index, m.Index - index).Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r").Replace("'", "\\'"));
                    else
                        sb.Append(_regSpaces.Replace(_text.Substring(index, m.Index - index).Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("'", "\\'")," "));
                    sb.Append(m.Value.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r").Replace("'", "\\'"));
                    index = m.Length + m.Index;
                    if (m.Value.StartsWith("<pre"))
                    {
                        int x = 0;
                        sb.Append(_ProcessPre(m.Index + m.Length, out x));
                        index = x;
                    }
                    m = _regTag.Match(_text, index);
                }
                if (index < _text.Length)
                    sb.Append(_regSpaces.Replace(_text.Substring(index).Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("'", "\\'")," "));
                return (sb.ToString().Trim() == "" ? "" : "ret+='" + sb.ToString() + "';");
            }
        }

        private string _ProcessPre(int index, out int x)
        {
            string ret = "";
            x = index;
            Match m = _regTag.Match(_text, x);
            while (m.Value != "")
            {
                ret += _text.Substring(x, m.Index - x).Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
                ret += m.Value;
                x = m.Length + m.Index;
                if (m.Value.StartsWith("<pre"))
                {
                    int y = 0;
                    ret += _ProcessPre(m.Index + m.Length, out y);
                    x += y;
                }
                else if (m.Value == "</pre>")
                {
                    return ret;
                }
                m = _regTag.Match(_text, index);
            }
            return ret;
        }
    }
}
