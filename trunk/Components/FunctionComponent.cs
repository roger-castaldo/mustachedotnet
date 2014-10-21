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

        public string ToJSCode(string dataVariable,bool compress)
        {
            StringBuilder sb = new StringBuilder();
            List<string> tmp = Utility.ParseCommandArguements(_text.Substring(1));
            if (tmp[0].StartsWith("$"))
                sb.AppendFormat("({0}==undefined ? undefined : {0}.{1}(",
                    Utility.CreateVariableString(dataVariable, tmp[0].Substring(1, tmp[0].LastIndexOf(".") - 1)),
                    tmp[0].Substring(tmp[0].LastIndexOf(".") + 1));
            else
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
            sb.Append((tmp[0]=="eval" ? "].join('')" : "")+(tmp[0].StartsWith("$") ? ")" : "")+")");
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
                    case '+':
                        if (buffer.Trim().Length > 0)
                        {
                            ret.Add(buffer.Trim());
                            buffer = "";
                        }
                        buffer += variable[x];
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
    }
}
