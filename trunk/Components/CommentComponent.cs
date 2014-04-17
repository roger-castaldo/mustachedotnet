using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal class CommentComponent : IComponent
    {
        public CommentComponent(string text)
        {
            _text = text.Trim();
        }

        private string _text;
        public string Text
        {
            get { return _text; }
        }

        public string ToJSCode(string dataVariable)
        {
            return (Text.Contains("\n") ?
                "/*" + _text.Replace("*/", "* /") + "*/" :
                "//" + _text);
        }
    }
}
