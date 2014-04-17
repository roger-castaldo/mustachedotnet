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
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable)
        {
            if (_text.StartsWith("& "))
                return string.Format("ret+=({0}==undefined ? '' : {0}.toString());", Utility.CreateVariableString(dataVariable, _text.Substring(2)));
            if (_text.EndsWith("}"))
                return string.Format("ret+=({0}==undefined ? '' : {0}.toString());", Utility.CreateVariableString(dataVariable, _text));
            else 
                return string.Format(@"var pre = document.createElement('pre');
    var text=document.createTextNode(({0}==undefined ? '' : {0}.toString()));
    pre.appendChild(text);
    ret+=pre.innerHTML;",
    Utility.CreateVariableString(dataVariable, _text.Trim('}')));
        }
    }
}
