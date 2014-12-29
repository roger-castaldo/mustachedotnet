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

        private const string _FUNCTION_LINE = "{1}function({0}){{var ret='';";
        private const string _START_CODE = @"if (this.pref==undefined){
    this.pref=function(txt){var pre = document.createElement('pre');
    var text=document.createTextNode(txt);
    pre.appendChild(text);
    return pre.innerHTML;}
}
if (this.cObj==undefined){
    this.cObj=function(obj){
        if (obj==undefined){
            return undefined;
        }else if (obj==null){
            return null;
        }else if(obj.cObj!=undefined){
            return obj;   
        }else{
            return {
                isArray:Array.isArray(obj)||obj.at!=undefined,
                length:obj.length,
                cObj:arguments.callee,
                _obj:obj,
                join:function(char){
                    if (this.isArray){
                        if (Array.isArray(this._obj)){
                            return this._obj.join(char);
                        }else{
                            var tmp = new Array();
                            for(var x=0;x<this._obj.length;x++){ tmp.push(this._obj.at(x));}
                            return tmp.join(char);
                        }
                    }
                },
                get:function(prop){
                    var ret = undefined;
                    if (prop=='length'){
                        ret = this.length;
                    }else{
                        if (this.isArray){
                            ret = (this._obj.at!=undefined ? (this._obj.at(prop)==undefined ? (this._obj[prop]!=undefined ? this._obj[prop] : (this._obj.get!=undefined ? this._obj.get(prop) : undefined)) : this._obj.at(prop)) : this._obj[prop]);
                            if (ret==undefined && typeof prop == 'string' && !this._obj.hasOwnProperty(prop)){
                                ret = new Array();
                                for(var x=0;x<this.length;x++){
                                    ret.push((this.get(x).get==undefined ? this.get(x) : this.get(x).get(prop)));
                                }
                                ret = (ret.length>0 && ret[0]==undefined ? undefined : (ret.length> 0 ? ret : undefined));
                            }
                        }else{
                            if (prop.indexOf('.')>0){
                                var tmp = this.get(prop.substring(0,prop.indexOf('.')));
                                ret =  (tmp==undefined ? undefined : (tmp.get==undefined ? tmp[prop.substring(prop.indexOf('.')+1)] : tmp.get(prop.substring(prop.indexOf('.')+1))));
                            }else{
                                ret = (this._obj[prop]!=undefined ? this._obj[prop] : (this._obj.get!=undefined ? this._obj.get(prop) : undefined));
                            }
                        }
                    }
                    return (ret==undefined ? undefined : (ret==null ? null : (Array.isArray(ret)||ret.toString()=='[object Object]' ? this.cObj(ret) : ret)));
                },
                toString:function(){
                    return this._obj.toString();
                }
            };   
        }
    }
}";
        private static readonly string _START_CODE_MIN = JSMinifier.Minify(_START_CODE);

        public static void GenerateCode(Stream[] sources, Stream destination, bool compress)
        {
            StreamWriter sw = new StreamWriter(destination);
            List<string> tmp = new List<string>();
            foreach (Stream str in sources)
            {
                StreamReader sr = new StreamReader(str);
                tmp.Add(sr.ReadToEnd());
                sr.Close();
            }
            sw.Write(GenerateCode(tmp, compress));
            sw.Flush();
        }

        public static void GenerateCode(List<Stream> sources, Stream destination, bool compress)
        {
            StreamWriter sw = new StreamWriter(destination);
            List<string> tmp = new List<string>();
            foreach (Stream str in sources)
            {
                StreamReader sr = new StreamReader(str);
                tmp.Add(sr.ReadToEnd());
                sr.Close();
            }
            sw.Write(GenerateCode(tmp, compress));
            sw.Flush();
        }

        public static void GenerateCode(Stream source, Stream destination,bool compress)
        {
            StreamReader sr = new StreamReader(source);
            StreamWriter sw = new StreamWriter(destination);
            string content = sr.ReadToEnd();
            sr.Close();
            sw.Write(GenerateCode(content,compress));
            sw.Flush();
        }

        public static string GenerateCode(string[] sources, bool compress)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in sources)
            {
                sb.AppendLine(GenerateCode(str, compress));
            }
            return sb.ToString();
        }

        public static string GenerateCode(List<string> sources, bool compress)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in sources)
            {
                sb.AppendLine(GenerateCode(str, compress));
            }
            return sb.ToString();
        }

        public static string GenerateCode(string source,bool compress)
        {
            if ((source==null ? "" : source) == "")
                return "";
            WrappedStringBuilder sb = new WrappedStringBuilder(compress);
            Parser parser = new Parser(source);
            foreach (Method m in parser.Methods)
            {
                string var = string.Format(DATA_VARIABLE_FORMAT, 1);
                sb.AppendLine(string.Format(_FUNCTION_LINE, var, (m.Name == null ? "" : m.Name + "=")));
                sb.AppendLine((compress ? _START_CODE_MIN : _START_CODE));
                sb.AppendLine(string.Format("{0}=this.cObj({0});", var));
                foreach (IComponent comp in m.Parts)
                    sb.AppendLine(comp.ToJSCode(var, compress));
                sb.AppendLine("return ret;}");
            }
            return sb.ToString();
        }
    }
}
