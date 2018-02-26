using Org.Reddragonit.MustacheDotNet.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.MustacheDotNet
{
    internal class Parser
    {
        private static readonly Regex _regPre = new Regex("<pre[^>]*>((?!</pre>).)+</pre>", RegexOptions.Compiled | RegexOptions.Singleline);

        private string _text;
        private int _curIndex;
        private string _buf;

        private List<Method> _methods;
        public List<Method> Methods { get { return _methods; } }

        public Parser(string text)
        {
            _text = text;
            _curIndex = 0; 
            _buf = "";
            _methods = _Parse();
        }

        private List<Method> _Parse()
        {
            List<Method> ret = new List<Method>();
            Method method = new Method();
            MatchCollection matches = _regPre.Matches(_text);
            string _curChars = "";
            while (_curIndex < _text.Length)
            {
                _curChars += _text[_curIndex];
                _curChars = (_curChars.Length > 2 ? _curChars.Substring(1) : _curChars);
                switch (_curChars)
                {
                    case "{{":
                        _buf = _buf.TrimEnd('{');
                        if (_buf.Length > 0)
                            method.Parts.Add(new StringConstant(_buf,_IsInPre(matches)));
                        _buf = "";
                        _curIndex++;
                        string commandText = _ExtractCommand();
                        switch (commandText[0])
                        {
                            case '#':
                            case '/':
                            case '^':
                                method.Parts.Add(new IfComponent(commandText));
                                break;
                            case '!':
                                commandText = commandText.Substring(1);
                                if (commandText.StartsWith("MethodName="))
                                {
                                    if (method.Name != null)
                                    {
                                        method.Parts = _RecurMergeIfs(null, 0, method.Parts);
                                        ret.Add(method);
                                        method = new Method();
                                    }
                                    method.Name = commandText.Substring("MethodName=".Length).Trim();
                                }
                                else
                                    method.Parts.Add(new CommentComponent(commandText));
                                break;
                            case '>':
                                method.Parts.Add(new SubTemplateComponent(commandText));
                                break;
                            case '=':
                                throw new Exception("Unable to change Delimeter.");
                                break;
                            case '%':
                                method.Parts.Add(new FunctionComponent(commandText));
                                break;
                            default:
                                method.Parts.Add(new VariableComponent(commandText));
                                break;
                        }
                        _curChars = _text[_curIndex-1].ToString();
                        if (_curIndex < _text.Length)
                        {
                            if (_text[_curIndex] == '}')
                                _curIndex++;
                        }
                        break;
                    default:
                        _buf += _text[_curIndex];
                        _curIndex++;
                        break;
                }
            }
            if (_buf != "")
                method.Parts.Add(new StringConstant(_buf, _IsInPre(matches)));
            if (method.Parts.Count > 0)
            {
                method.Parts = _RecurMergeIfs(null, 0, method.Parts);
                ret.Add(method);
            }
            return ret;
        }

        private bool _IsInPre(MatchCollection matches)
        {
            bool ret = false;
            foreach (Match m in matches)
            {
                if (m.Index <= _curIndex && m.Index + m.Length >= _curIndex)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        private List<IComponent> _RecurMergeIfs(string ifText,int index,List<IComponent> comps)
        {
            List<IComponent> ret = new List<IComponent>();
            for (int x = index; x < comps.Count; x++)
            {
                if (comps[x] is IfComponent)
                {
                    switch (comps[x].Text[0])
                    {
                        case '#':
                        case '^':
                            if (comps[x].Text != "#else#")
                            {
                                IfComponent ifc = (IfComponent)comps[x];
                                ifc.Children = _RecurMergeIfs(ifc.Text.Substring(1), x + 1, comps);
                                ret.Add(ifc);
                                x += ifc.Length;
                            }
                            else
                                ret.Add(comps[x]);
                            break;
                        case '/':
                            if ((ifText == null ? "" : ifText) == comps[x].Text.Substring(1))
                                return ret;
                            else
                                ret.Add(comps[x]);
                            break;
                    }
                }else
                    ret.Add(comps[x]);
            }
            return ret;
        }

        private string _ExtractCommand()
        {
            string _curChars = "";
            string ret="";
            while (_curIndex < _text.Length)
            {
                _curChars += _text[_curIndex];
                _curChars = (_curChars.Length > 2 ? _curChars.Substring(1) : _curChars);
                if (_curChars == "}}")
                {
                    ret = ret.TrimEnd('}');
                    _curIndex++;
                    break;
                }
                else
                {
                    ret += _text[_curIndex];
                    _curIndex++;
                }
            }
            return ret;
        }
    }
}
