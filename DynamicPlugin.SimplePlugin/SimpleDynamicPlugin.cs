using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicPlugin.Core;

namespace DynamicPlugin.SimplePlugin
{
    public class SimpleDynamicPlugin:IPlugin
    {
        public void GetInfo()
        {
            Console.WriteLine("Version 4");
        }

        public void Dispose()
        {
        }
    }
}
