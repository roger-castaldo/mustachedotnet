using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet
{
    internal class WrappedStringBuilder
    {
        private StringBuilder _sb;
        private bool _minimize;

        public WrappedStringBuilder(bool minimize)
        {
            _sb = new StringBuilder();
            _minimize = minimize;
        }

        public string ToString()
        {
            return _sb.ToString();
        }

        public void AppendLine(string line)
        {
            if (_minimize)
                _sb.Append(line);
            else
                _sb.AppendLine(line);
        }

        public void AppendFormat(string format, object par1, object par2)
        {
            _sb.AppendFormat(format, par1, par2);
        }

        public void Append(object obj)
        {
            _sb.Append(obj);
        }
    }
}
