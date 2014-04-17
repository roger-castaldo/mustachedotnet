using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class StringConstant : IComponent
    {
        private string _text;

        public StringConstant(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable)
        {
            return "ret+='" + _text.Trim().Replace("'", "\\'").Replace("\n","\\n").Replace("\r","\\r").Replace("\t","\\t") + "';";
        }
    }
}
