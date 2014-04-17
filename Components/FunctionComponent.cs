using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class FunctionComponent : IComponent
    {
        public FunctionComponent(string text)
        {
            _text = text;
        }

        private string _text;
        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable)
        {
            StringBuilder sb = new StringBuilder();
            string buf = "";
            List<string> tmp = new List<string>();
            for (int x = 1; x < _text.Length; x++)
            {
                switch (_text[x])
                {
                    case ',':
                        if (buf != "")
                            tmp.Add(buf);
                        buf = "";
                        break;
                    case '\'':
                    case '"':
                        buf += _ProcessQuote(ref x,_text[x]);
                        break;
                    default:
                        buf += _text[x];
                        break;
                }
            }
            if (buf != "")
                tmp.Add(buf);
            sb.Append(tmp[0]+"(");
            for (int x = 1; x < tmp.Count; x++)
            {
                switch(tmp[x][0]){
                    case '\'':
                    case '"':
                        sb.Append(tmp[x] + (x + 1 == tmp.Count ? "" : ","));
                        break;
                    default:
                        sb.Append(Utility.CreateVariableString(dataVariable,tmp[x])+(x+1==tmp.Count ? "" : ","));
                        break;
                }
            }
            sb.AppendLine(")");
            return string.Format("ret+=({0}==undefined ? '' : {0}.toString());", sb);
        }

        private string _ProcessQuote(ref int x,char quote)
        {
            string ret = _text[x].ToString();
            x++;
            while (_text[x]!=quote)
            {
                ret += _text[x];
                x++;
            }
            ret += _text[x];
            return ret;
        }
    }
}
