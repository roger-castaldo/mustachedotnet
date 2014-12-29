using Org.Reddragonit.MustacheDotNet.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.MustacheDotNet
{
    internal class Method
    {
        private string _name=null;
        public string Name { 
            get { return _name; } 
            set{_name=value;}
        }

        private List<IComponent> _parts;
        public List<IComponent> Parts
        {
            get { return _parts; }
            set { _parts = value; }
        }

        public Method()
        {
            _parts = new List<IComponent>();
        }
    }
}
