﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.MustacheDotNet
{
    internal static class Utility
    {
        private static readonly Regex _RegNum = new Regex("\\d+", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private static readonly Regex _RegEscape = new Regex("%esc%$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        
        public static string CreateVariableString(string dataVariable, string variableName)
        {
            variableName = variableName.Trim();
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
                return CreateVariableString(string.Format(Generator.DATA_VARIABLE_FORMAT, 1), variableName.Substring("global:".Length));
            else if (variableName == "$index")
                return "x" + (int.Parse(_RegNum.Match(dataVariable).Value)-1).ToString();
            else if (variableName.StartsWith("parent:"))
            {
                variableName = variableName.Substring("parent:".Length);
                int val = int.Parse(dataVariable.Substring(string.Format(Generator.DATA_VARIABLE_FORMAT, "").Length));
                return CreateVariableString(string.Format(Generator.DATA_VARIABLE_FORMAT, val - 1), variableName);
            }
            else
            {
                Match m = _RegEscape.Match(variableName);
                if (m.Success)
                {
                    variableName = variableName.Substring(0, m.Index);
                    string comm = CreateVariableString(dataVariable, variableName);
                    return (comm.StartsWith("!") ? "!escAll(" + comm.Substring(1) + ")" : "escAll(" + comm + ")");
                }else
                    return string.Format("{2}({0}==undefined ? undefined : {0}.get('{1}'))",
                            dataVariable,
                            (variableName.StartsWith("!") ? variableName.Substring(1) : variableName),
                            (variableName.StartsWith("!") ? "!" : ""));
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
                            ret.Add(buf.Trim());
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
                ret.Add(buf.Trim());
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

        internal static string Format(string format, object[] data)
        {
            format = format.Replace("{", "{{").Replace("}", "}}");
            for (int x = 0; x < data.Length; x++)
                format = format.Replace("$" + x.ToString(), "{" + x.ToString() + "}");
            return string.Format(format, data);
        }
    }
}
