using Org.Reddragonit.MustacheDotNet.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet
{
    internal class Parser
    {
        private string _text;
        private int _curIndex;
        private string _buf;
        private List<IComponent> _parts;
        public List<IComponent> Parts
        {
            get { return _parts; }
        }
        private string _methodName=null;
        public string MethodName
        {
            get { return _methodName; }
        }

        public Parser(string text)
        {
            _text = text;
            _curIndex = 0; 
            _buf = "";
            _parts = _Parse();
        }

        private List<IComponent> _Parse()
        {
            List<IComponent> ret = new List<IComponent>();
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
                            ret.Add(new StringConstant(_buf));
                        _buf = "";
                        _curIndex++;
                        string commandText = _ExtractCommand();
                        switch (commandText[0])
                        {
                            case '#':
                            case '/':
                            case '^':
                                ret.Add(new IfComponent(commandText));
                                break;
                            case '!':
                                commandText = commandText.Substring(1);
                                if (commandText.StartsWith("MethodName="))
                                    _methodName = commandText.Substring("MethodName=".Length).Trim();
                                else
                                    ret.Add(new CommentComponent(commandText));
                                break;
                            case '>':
                                ret.Add(new SubTemplateComponent(commandText));
                                break;
                            case '=':
                                throw new Exception("Unable to change Delimeter.");
                                break;
                            case '%':
                                ret.Add(new FunctionComponent(commandText));
                                break;
                            default:
                                ret.Add(new VariableComponent(commandText));
                                break;
                        }
                        if (_text[_curIndex] == '}')
                            _curIndex++;
                        break;
                    default:
                        _buf += _text[_curIndex];
                        _curIndex++;
                        break;
                }
            }
            if (_buf != "")
                ret.Add(new StringConstant(_buf));
            ret = _RecurMergeIfs(0,ret);
            return ret;
        }

        private List<IComponent> _RecurMergeIfs(int index,List<IComponent> comps)
        {
            List<IComponent> ret = new List<IComponent>();
            for (int x = index; x < comps.Count; x++)
            {
                switch (comps[x].Text[0])
                {
                    case '#':
                    case '^':
                        IfComponent ifc = (IfComponent)comps[x];
                        ifc.Children = _RecurMergeIfs(x + 1, comps);
                        ret.Add(ifc);
                        x += ifc.Children.Count+1;
                        break;
                    case '/':
                        x = comps.Count;
                        break;
                    default:
                        ret.Add(comps[x]);
                        break;
                }
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
