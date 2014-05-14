using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class SubTemplateComponent : IComponent
    {
        public SubTemplateComponent(string text)
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
            if (_text.Contains(","))
            {
                StringBuilder sb = new StringBuilder();
                List<string> tmp = Utility.ParseCommandArguements(_text.Substring(1));
                sb.AppendFormat("ret+={0}(", tmp[0]);
                if (tmp.Count == 2)
                    sb.Append(Utility.CreateVariableString(dataVariable, (tmp[1].StartsWith("\"") || tmp[1].StartsWith("'") ? tmp[1] : Utility.CreateVariableString(dataVariable, tmp[1]))));
                else
                {
                    sb.Append("{");
                    for (int x = 1; x < tmp.Count; x += 2)
                        sb.AppendFormat("{0}:{1}{2}", new object[]{
                            tmp[x],
                            (tmp[x+1].StartsWith("\"")||tmp[x+1].StartsWith("'") ? tmp[x+1] : Utility.CreateVariableString(dataVariable,tmp[x+1])),
                            (x+2<tmp.Count ? "," : "}")
                        });
                }
                sb.AppendFormat(");");
                return sb.ToString();
            }else
                return string.Format("ret+={0}({1});", _text.Substring(1), dataVariable);
        }
    }
}
