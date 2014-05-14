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
            sb.Append(tmp[0]+"("+(tmp[0]=="eval" ? "[" : ""));
            for (int x = 1; x < tmp.Count; x++)
            {
                switch(tmp[x][0]){
                    case '\'':
                    case '"':
                        List<string> splt = _SplitVariable(tmp[x]);
                        foreach(string str in splt){
                            switch(str[0]){
                                case '\'':
                                case '"':
                                    sb.Append(str);
                                    break;
                                default:
                                    switch(str[0]){
                                        case '+':
                                            sb.Append(str[0]);
                                            if (str.Length > 1)
                                                sb.Append(Utility.CreateVariableString(dataVariable, str.Substring(1)));
                                            break;
                                        default:
                                            throw new Exception("Unable to parse function code");
                                            break;
                                    }
                                    break;
                            }
                        }
                        sb.Append((x + 1 == tmp.Count ? "" : ","));
                        break;
                    default:
                        sb.Append(Utility.CreateVariableString(dataVariable, tmp[x]) + (x + 1 == tmp.Count ? "" : ","));
                        break;
                }
            }
            sb.Append((tmp[0]=="eval" ? "].join('')" : "")+")");
            return string.Format("ret+=({0}==undefined ? '' : {0}.toString());", sb);
        }

        private List<string> _SplitVariable(string variable)
        {
            List<string> ret = new List<string>();
            string buffer = "";
            for (int x = 0; x < variable.Length; x++)
            {
                switch (variable[x])
                {
                    case '\'':
                    case '"':
                        if (buffer.Trim().Length > 0)
                            ret.Add(buffer.Trim());
                        buffer = variable[x].ToString();
                        x++;
                        while (variable[x] != buffer[0])
                        {
                            buffer += variable[x];
                            x++;
                        }
                        buffer += variable[x];
                        ret.Add(buffer);
                        buffer = "";
                        break;
                    default:
                        buffer+=variable[x];
                        break;

                }
            }
            if (buffer.Trim().Length > 0)
                ret.Add(buffer.Trim());
            return ret;
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
