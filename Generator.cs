﻿using Org.Reddragonit.MustacheDotNet.Components;
using Org.Reddragonit.MustacheDotNet.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet
{
    public static class Generator
    {
        internal const string DATA_VARIABLE_FORMAT = "data{0}";

        private const string _FUNCTION_LINE = "{1}function({0}){{var ret='';";
        private const string _START_CODE = @"function pref(txt){
        var pre = document.createElement('pre');
        var text=document.createTextNode(txt);
        pre.appendChild(text);
        return pre.innerHTML;
    }
    function cObj(obj){
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
    function escAll(src){
        if (src==undefined){
            return src;
        }else if (typeof src === 'string' || src instanceof String){
            if (this.escapes==undefined){ this.escapes = [['\\','\\\\'],['""','\""'],['\'','\\\'']]; }
            var ret = src;
            for(var x=0;x<this.escapes.length;x++){
                var lastIndexOf = ret.indexOf(this.escapes[x][0]);
                while (lastIndexOf>=0){
                    ret = ret.substring(0,lastIndexOf)+this.escapes[x][1]+ret.substring(lastIndexOf+this.escapes[x][0].length);
                    lastIndexOf = ret.indexOf(this.escapes[x][0],lastIndexOf+this.escapes[x][1].length);
                }
            }
            return ret;
        }
        return src;
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
            WrappedStringBuilder sb = new WrappedStringBuilder(compress);
            sb.AppendLine("(function(){"+(compress ? _START_CODE_MIN : _START_CODE));
            foreach (string str in sources)
                sb.AppendLine(_GenerateCode(str, compress, false));
            sb.AppendLine("}).call(this);");
            return sb.ToString();
        }

        public static string GenerateCode(List<string> sources, bool compress)
        {
            WrappedStringBuilder sb = new WrappedStringBuilder(compress);
            sb.AppendLine("(function(){" + (compress ? _START_CODE_MIN : _START_CODE));
            foreach (string str in sources)
                sb.AppendLine(_GenerateCode(str, compress,false));
            sb.AppendLine("}).call(this);");
            return sb.ToString();
        }

        public static string GenerateCode(string source, bool compress)
        {
            return _GenerateCode(source, compress, true);
        }

        private static string _GenerateCode(string source,bool compress,bool includeFunctions)
        {
            if ((source==null ? "" : source) == "")
                return "";
            WrappedStringBuilder sb = new WrappedStringBuilder(compress);
            Parser parser = new Parser(source);
            if (includeFunctions)
                sb.AppendLine("(function(){" + (compress ? _START_CODE_MIN : _START_CODE));
            Hashtable nspaces = new Hashtable();
            foreach (Method m in parser.Methods)
            {
                if ((m.Name==null ? "" : m.Name).Contains(".")){
                    Hashtable ht = nspaces;
                    foreach (string str in m.Name.Split('.'))
                    {
                        if (!m.Name.EndsWith(str))
                        {
                            if (!ht.ContainsKey(str))
                                ht.Add(str, new Hashtable());
                            ht = (Hashtable)ht[str];
                        }
                    }
                }
            }
            if (nspaces.Count > 0)
            {
                foreach (string str in nspaces.Keys)
                {
                    sb.AppendLine(string.Format("window.{0}=window.{0}||{{}};", str));
                    _RecurAddNamespaces(sb,str, (Hashtable)nspaces[str]);
                }
            }
            foreach (Method m in parser.Methods)
            {
                string var = string.Format(DATA_VARIABLE_FORMAT, 1);
                sb.AppendLine(string.Format(_FUNCTION_LINE, var, (m.Name == null ? "" : m.Name + "=")));
                sb.AppendLine(string.Format("{0}=cObj({0});", var));
                foreach (IComponent comp in m.Parts)
                    sb.AppendLine(comp.ToJSCode(var, compress));
                sb.AppendLine("return ret;};");
            }
            if (includeFunctions)
                sb.AppendLine("}).call(this);");
            return sb.ToString();
        }

        private static void _RecurAddNamespaces(WrappedStringBuilder sb,string basePath, Hashtable nspaces)
        {
            foreach (string str in nspaces.Keys)
            {
                sb.AppendLine(string.Format("{0}.{1}={0}.{1}||{{}};", basePath, str));
                _RecurAddNamespaces(sb, basePath + "." + str, (Hashtable)nspaces[str]);
            }
        }
    }
}
