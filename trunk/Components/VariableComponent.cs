using Org.Reddragonit.MustacheDotNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class VariableComponent : IComponent
    {
        private string _text;

        private static string[] _CODE_LINES;
        private static string[] _CODE_LINES_COMPRESS;

        static VariableComponent()
        {
            _CODE_LINES = new string[]{
            @"if (($0==undefined ? false : $0.isArray)){{
        ret+=$0.join('$1');
    }}",
       @"else if (($0==undefined ? false : $0.isArray)){{
        for(var i=0;i<$0.length;i++){{
            ret+=($0.get(i)==undefined ? undefined : ($0.get(i).get('$1')!=undefined ? $0.get(i]).get('$1') : undefined));
            if (i+1<$0.length){{
                ret+='$2';
            }}
        }}
    }}",
       @"else{{
        ret+=($0==undefined ? '' : ($0==null ? '' : $0.toString()+'$1'));
    }}",
       @"if (($0==undefined ? false : $0.isArray)){{
        ret+=this.pref($0.join('$1'));
    }}",
       @"else if (($0==undefined ? false : $0.isArray)){{
        for(var i=0;i<$0.length;i++){{
            ret+=this.pref(($0.get(i)==undefined ? undefined : $0.get(i).get('$1')!=undefined)+(i+1<$0.length ? '$2' : ''));
        }}
    }}",
       @"else{{
        ret+=this.pref(($0==undefined ? '' : ($0==null ? '' : $0.toString()+'$1')));
    }}"
            };
            _CODE_LINES_COMPRESS = new string[]{
                Utility.Format(JSMinifier.Minify(_CODE_LINES[0]),new object[]{"{0}","{1}","{2}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[1]),new object[]{"{0}","{1}","{2}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[2]),new object[]{"{0}","{1}","{2}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[3]),new object[]{"{0}","{1}","{2}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[4]),new object[]{"{0}","{1}","{2}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[5]),new object[]{"{0}","{1}","{2}"}),
            };
            _CODE_LINES[0] = Utility.Format(_CODE_LINES[0], new object[] { "{0}", "{1}", "{2}" });
            _CODE_LINES[1] = Utility.Format(_CODE_LINES[1], new object[] { "{0}", "{1}", "{2}" });
            _CODE_LINES[2] = Utility.Format(_CODE_LINES[2], new object[] { "{0}", "{1}", "{2}" });
            _CODE_LINES[3] = Utility.Format(_CODE_LINES[3], new object[] { "{0}", "{1}", "{2}" });
            _CODE_LINES[4] = Utility.Format(_CODE_LINES[4], new object[] { "{0}", "{1}", "{2}" });
            _CODE_LINES[5] = Utility.Format(_CODE_LINES[5], new object[] { "{0}", "{1}", "{2}" });
        }

        public VariableComponent(string text)
        {
            _text = text;
            _text = (_text.StartsWith("{") ? "& " + _text.Substring(1) : (_text.EndsWith("}") ? "& "+_text.Substring(0,_text.Length-1) : _text));
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable,bool compress)
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
                    ret = string.Format((compress ? _CODE_LINES_COMPRESS[0] : _CODE_LINES[0]), new object[] { var, repeater });
                    if (subVar != "")
                        ret += string.Format((compress ? _CODE_LINES_COMPRESS[1] : _CODE_LINES[1]), new object[] { subVar, subVarField, repeater });
                    ret += string.Format((compress ? _CODE_LINES_COMPRESS[2] : _CODE_LINES[2]), new object[] { var, repeater });
                }
                else
                {
                    ret = string.Format((compress ? _CODE_LINES_COMPRESS[3] : _CODE_LINES[3]), new object[] { var, repeater });
                    if (subVar != "")
                        ret += string.Format((compress ? _CODE_LINES_COMPRESS[4] : _CODE_LINES[4]), new object[] { subVar, subVarField, repeater });
                    ret += string.Format((compress ? _CODE_LINES_COMPRESS[5] : _CODE_LINES[5]), new object[] { var, repeater });
                }
                return ret;
            }else if (_text.StartsWith("& "))
                return string.Format("ret+=({0}==undefined ? '' : {0}.toString());", Utility.CreateVariableString(dataVariable, _text.Substring(2)));
            else 
                return string.Format(@"ret+=this.pref(({0}==undefined ? '' : {0}.toString()));",
                Utility.CreateVariableString(dataVariable, _text.Trim('}')));
        }
    }
}
