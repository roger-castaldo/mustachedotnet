using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class VariableComponent : IComponent
    {
        private string _text;

        public VariableComponent(string text)
        {
            _text = text;
            _text = (_text.StartsWith("{") ? "& " + _text.Substring(1) : (_text.EndsWith("}") ? "& "+_text.Substring(0,_text.Length-1) : _text));
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable)
        {
            if (_text.Contains(","))
            {
                string subVar = _text.Substring(0, _text.IndexOf(",")).Substring((_text.StartsWith("& ") ? 2 : 0));
                string var = Utility.CreateVariableString(dataVariable,subVar);
                string subVarField = (subVar.Contains(".") ? subVar.Substring(subVar.IndexOf(".") + 1) : "");
                subVar = (subVar.Contains(".") ? Utility.CreateVariableString(dataVariable, subVar.Substring(0, subVar.LastIndexOf("."))) : "");
                string repeater = _text.Substring(_text.IndexOf(",") + 1).TrimEnd('}').Replace("\n","\\n").Replace("\t","\\t").Replace("\r","\\r").Replace("'","\\'");
                string ret = "";
                if (_text.StartsWith("& ") || _text.EndsWith("}"))
                {
                    ret = string.Format(@"if (Array.isArray({0})){{
    ret+={0}.join('{1}');
}}", new object[] { var, repeater });
                    if (subVar != "")
                        ret += string.Format(@"else if (Array.isArray({0})){{
    for(var i=0;i<{0}.length;i++){{
        ret+=({0}[i]==undefined ? undefined : ({0}[i].{1}!=undefined ? {0}[i].{1} : ({0}[i].get != undefined ? {0}[i].get('{1}') : undefined)));
        if (i+1<{0}.length){{
            ret+='{2}';
        }}
    }}
}}else if ({0}.at!=undefined){{
    for(var i=0;i<{0}.length;i++){{
        ret+=({0}.at(i)==undefined ? undefined : ({0}.at(i).{1}!=undefined ? {0}.at(i).{1} : ({0}.at(i).get != undefined ? {0}.at(i).get('{1}') : undefined)));
        if (i+1<{0}.length){{
            ret+='{2}';
        }}
    }}
}}", new object[] { subVar,subVarField,repeater });
                    ret += string.Format(@"else{{
    ret+=({0}==undefined ? '' : ({0}==null ? '' : {0}.toString()+'{1}'));
}}",new object[]{var,repeater});
                }
                else
                {
                    ret = string.Format(@"if (Array.isArray({0})){{
    ret+=pref(({0}==undefined ? '' : {0}.join('{1}')));
}}", new object[] { var, repeater });
                    if (subVar != "")
                        ret += string.Format(@"else if (Array.isArray({0})){{
    for(var i=0;i<{0}.length;i++){{
        ret+=pref({0}[i]==undefined ? undefined : ({0}[i].{1}!=undefined ? {0}[i].{1} : ({0}[i].get != undefined ? {0}[i].get('{1}') : undefined)))+(i+1<{0}.length ? '{2}' : ''));
    }}
}}else if ({0}.at!=undefined){{
    for(var i=0;i<{0}.length;i++){{
        ret+=pref(({0}.at(i)==undefined ? undefined : ({0}.at(i).{1}!=undefined ? {0}.at(i).{1} : ({0}.at(i).get != undefined ? {0}.at(i).get('{1}') : undefined)))+(i+1<{0}.length ? '{2}' : ''));
    }}
}}", new object[] { subVar, subVarField, repeater });
                    ret += string.Format(@"else{{
    ret+=pref(({0}==undefined ? '' : ({0}==null ? '' : {0}.toString()+'{1}')));
}}", new object[] { var, repeater });
                }
                return ret;
            }else if (_text.StartsWith("& "))
                return string.Format("ret+=({0}==undefined ? '' : {0}.toString());", Utility.CreateVariableString(dataVariable, _text.Substring(2)));
            else 
                return string.Format(@"ret+=pref(({0}==undefined ? '' : {0}.toString()));",
                Utility.CreateVariableString(dataVariable, _text.Trim('}')));
        }
    }
}
