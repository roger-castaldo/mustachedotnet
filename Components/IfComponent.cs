using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class IfComponent : IComponent
    {
        private static readonly Regex _RegNum = new Regex("\\d+", RegexOptions.Compiled | RegexOptions.ECMAScript);

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

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable)
        {
            string ret = "";
            int num = int.Parse(_RegNum.Match(dataVariable).Value);
            dataVariable = dataVariable.Replace(num.ToString(), "");
            string subCode = "";
            foreach (IComponent comp in _children)
                subCode += comp.ToJSCode(dataVariable+(num+1).ToString());
            ret += string.Format(
@"if ({0}{1}){{
    if (Array.isArray({1})){{
        for(var x{2}=0;x{2}<{1}.length;x{2}++){{
            {3}=({1}.at==undefined ? {1}[x{2}] : {1}.at(x{2}));
            {4}
        }}
    }}else{{
        {3}={1};
        {4}
    }}
}}", new object[]{
       (_text[0]=='#' ? "" : "!"),
       Utility.CreateVariableString(dataVariable+num.ToString(), _text.Substring(1)),
       num,
       dataVariable+(num+1).ToString(),
       subCode
 });
            return ret;
        }
    }
}
