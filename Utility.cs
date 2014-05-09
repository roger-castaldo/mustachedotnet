using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.MustacheDotNet
{
    internal static class Utility
    {
        
        public static string CreateVariableString(string dataVariable, string variableName)
        {
            if (variableName == "")
                return dataVariable;
            else if (variableName.StartsWith("$1"))
            {
                if (variableName.Contains("."))
                    return CreateVariableString(dataVariable, variableName.Substring(variableName.IndexOf(".") + 1));
                else
                    return dataVariable;
            }
            else if (variableName.StartsWith("global:"))
                return CreateVariableString(string.Format(Generator.DATA_VARIABLE_FORMAT, 1),variableName.Substring("global:".Length));
            else if (variableName.StartsWith("parent:"))
            {
                variableName = variableName.Substring("parent:".Length);
                int val = int.Parse(dataVariable.Substring(string.Format(Generator.DATA_VARIABLE_FORMAT,"").Length));
                return CreateVariableString(string.Format(Generator.DATA_VARIABLE_FORMAT, val -1), variableName);
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
