using Org.Reddragonit.MustacheDotNet.Components;
using Org.Reddragonit.MustacheDotNet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet
{
    public static class Generator
    {
        internal const string DATA_VARIABLE_FORMAT = "data{0}";

        public static void GenerateCode(Stream source, Stream destination,bool compress)
        {
            StreamReader sr = new StreamReader(source);
            StreamWriter sw = new StreamWriter(destination);
            string content = sr.ReadToEnd();
            sr.Close();
            sw.Write(GenerateCode(content,compress));
            sw.Flush();
        }

        public static string GenerateCode(string source,bool compress)
        {
            if (source == "")
                return "";
            StringBuilder sb = new StringBuilder();
            string var = string.Format(DATA_VARIABLE_FORMAT,1);
            Parser parser = new Parser(source);
            sb.AppendLine(string.Format("{1}function({0}){{var ret='';", var,(parser.MethodName==null ? "" : parser.MethodName+"=")));
            sb.AppendLine(@"var pref=function(txt){var pre = document.createElement('pre');
    var text=document.createTextNode(txt);
    pre.appendChild(text);
    return pre.innerHTML;}");
            foreach (IComponent comp in parser.Parts)
                sb.AppendLine(comp.ToJSCode(var));
            sb.AppendLine("return ret;}");
            return (compress ? JSMinifier.Minify(sb.ToString()) : sb.ToString());
        }
    }
}
