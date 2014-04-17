using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet.Components
{
    internal interface IComponent
    {
        string Text { get; }
        string ToJSCode(string dataVariable);
    }
}
