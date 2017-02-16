using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace HTML_Parser.Common
{
    public static class DependencyResolver
    {
        static IContainer c;
        static Registry r = new Registry();
        public static void Register<Abstraction, Implementation>() where Implementation : Abstraction
        {
            r.For<Abstraction>().Use<Implementation>();
            c = new Container(x => x.AddRegistry(r));
        }
        public static T GetInstance<T>()
        {
            return c.GetInstance<T>();
        }
    }
}
