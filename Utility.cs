using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet
{
    internal static class Utility
    {
        public static string CreateVariableString(string dataVariable, string variableName)
        {
            if (variableName.StartsWith("$1"))
            {
                if (variableName.Contains("."))
                    return CreateVariableString(dataVariable,variableName.Substring(variableName.IndexOf(".")+1));
                else
                    return dataVariable;
            }
            else
            {
                if (variableName.Contains("."))
                {
                    return CreateVariableString(string.Format("({0}==undefined ? undefined : ({0}.{1}!=undefined ? {0}.{1} : ({0}.get != undefined ? {0}.get('{1}') : undefined)))",
                        dataVariable,
                        variableName.Substring(0, variableName.IndexOf("."))), variableName.Substring(variableName.IndexOf(".") + 1));
                }
                else
                {
                    return string.Format("({0}==undefined ? undefined : ({0}.{1}!=undefined ? {0}.{1} : ({0}.get != undefined ? {0}.get('{1}') : undefined)))",
                        dataVariable,
                        variableName);
                }
            }
        }
    }
}
