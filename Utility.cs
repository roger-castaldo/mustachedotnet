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

        internal static List<string> ParseCommandArguements(string text)
        {
            string buf = "";
            List<string> ret = new List<string>();
            for (int x = 0; x < text.Length; x++)
            {
                switch (text[x])
                {
                    case ',':
                        if (buf != "")
                            ret.Add(buf);
                        buf = "";
                        break;
                    case '\'':
                    case '"':
                        buf += _ProcessQuote(ref x,text, text[x]);
                        break;
                    default:
                        buf += text[x];
                        break;
                }
            }
            if (buf != "")
                ret.Add(buf);
            return ret;
        }

        internal static string _ProcessQuote(ref int x,string text, char quote)
        {
            string ret = text[x].ToString();
            x++;
            while (text[x] != quote)
            {
                if (text[x]=='\\' && text[x+1]==quote)
                {
                    ret += "\\" + quote.ToString();
                    x++;
                }
                else
                    ret += text[x];
                x++;
            }
            ret += text[x];
            return ret;
        }
    }
}
