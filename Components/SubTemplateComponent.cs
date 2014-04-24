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
            return string.Format("ret+={0}({1});", _text.Substring(1), dataVariable);
        }
    }
}
