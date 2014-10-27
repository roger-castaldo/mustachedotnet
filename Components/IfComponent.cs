using Org.Reddragonit.MustacheDotNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class IfComponent : IComponent
    {
        private static readonly Regex _RegNum = new Regex("\\d+", RegexOptions.Compiled | RegexOptions.ECMAScript);

        private static string[] _CODE_LINES;
        private static string[] _CODE_LINES_COMPRESS;

        static IfComponent()
        {
            _CODE_LINES = new string[]{
            @"var tmp$3=$1);
if ($0(tmp$3==undefined ? false : (tmp$3==null ? false : tmp$3))){{
$2
}}$4",
          @"var tmp$2=$1;
    if ($0(tmp$2==undefined ? false : (tmp$2==null ? false : (tmp$2.isArray ? tmp$2.length>0 : tmp$2)))){{
        if ((tmp$2!=undefined&&tmp$2!=null ? tmp$2.isArray : false)){{
            for(var x$2=0;x$2<tmp$2.length;x$2++){{
                var $3=tmp$2.get(x$2);
                $4
            }}
        }}else{{
            var $3=tmp$2;
            $4
        }}
    }}",
       @"else{{
        if ((tmp$2!=undefined&&tmp$2!=null ? tmp$2.isArray : false)){{
            for(var x$2=0;x$2<tmp$2.length;x$2++){{
                var $3=tmp$2.get(x$2);
                $4
            }}
        }}else{{
            var $3=tmp$2;
            $4
        }}
    }}"
            };
            _CODE_LINES_COMPRESS = new string[]{
                Utility.Format(JSMinifier.Minify(_CODE_LINES[0]).Trim(),new object[]{"{0}","{1}","{2}","{3}","{4}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[1]).Trim(),new object[]{"{0}","{1}","{2}","{3}","{4}"}),
                Utility.Format(JSMinifier.Minify(_CODE_LINES[2]).Trim(),new object[]{"{0}","{1}","{2}","{3}","{4}"})
            };
            _CODE_LINES[0] = Utility.Format(_CODE_LINES[0], new object[] { "{0}", "{1}", "{2}", "{3}", "{4}" });
            _CODE_LINES[1] = Utility.Format(_CODE_LINES[1], new object[] { "{0}", "{1}", "{2}", "{3}", "{4}" });
            _CODE_LINES[2] = Utility.Format(_CODE_LINES[2], new object[] { "{0}", "{1}", "{2}", "{3}", "{4}" });
        } 

        private string _text;
        private List<IComponent> _children;
        public List<IComponent> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public IfComponent(string text)
        {
            _children = new List<IComponent>();
            _text = text;
        }

        public int Length
        {
            get
            {
                int ret = 0;
                foreach (IComponent child in _children)
                {
                    ret++;
                    if (child is IfComponent && child.Text!="#else#")
                        ret += ((IfComponent)child).Length;
                }
                return ret+1;
            }
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable,bool compress)
        {
            string ret = "";
            int num = int.Parse(_RegNum.Match(dataVariable).Value);
            dataVariable = dataVariable.Replace(num.ToString(), "");
            string subCode = "";
            string elseCode = "";
            bool inElse = false;
            string varName = dataVariable + (_text.Substring(1).StartsWith("%") ? num : num + 1).ToString();
            foreach (IComponent comp in _children)
            {
                if (comp.Text == "#else#")
                {
                    inElse = true;
                }
                else
                {
                    if (inElse)
                        elseCode += comp.ToJSCode(varName,compress);
                    else
                        subCode += comp.ToJSCode(varName,compress);
                }
            }
            if (_text.Substring(1).StartsWith("%"))
            {
                
                FunctionComponent fc = new FunctionComponent(_text.Substring(1));
                ret = fc.ToJSCode(dataVariable + num.ToString(),compress);
                ret = ret.Substring(5);
                ret = ret.Substring(0, ret.LastIndexOf("==undefined ? ''"));
                ret = string.Format((compress ? _CODE_LINES_COMPRESS[0] : _CODE_LINES[0]),
   new object[]{(_text[0] == '#' ? "" : "!"),
   ret,
   subCode,
   num,
   (elseCode=="" ? "" : "else{"+elseCode+"}")});
            }
            else
            {
                ret = string.Format((compress ? _CODE_LINES_COMPRESS[1] : _CODE_LINES[1]), new object[]{
       (_text[0]=='#' ? "" : "!"),
       Utility.CreateVariableString(dataVariable+num.ToString(), _text.Substring(1)),
       num,
       dataVariable+(num+1).ToString(),
       subCode
 });
                if (elseCode != "")
                {
                    ret += string.Format((compress ? _CODE_LINES_COMPRESS[2] : _CODE_LINES[2]), new object[]{
       (_text[0]=='#' ? "" : "!"),
       Utility.CreateVariableString(dataVariable+num.ToString(), _text.Substring(1)),
       num,
       dataVariable+(num+1).ToString(),
       elseCode
 });
                }
            }
            return ret;
        }
    }
}
