using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicPlugin.Core;

namespace DynamicPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            PluginManager pluginManager = new PluginManager();

            pluginManager.RegisterFileChange();
            IPlugin currentPlugin=null;
            var temp = string.Empty;

            do
            {
                temp = Console.ReadLine() ?? string.Empty;

                var cmdargs = temp.Split(' ');
                if (cmdargs.Length > 1)
                {
                    switch (cmdargs[0].ToLower())
                    {
                        case "reg":
                            var pathFile = cmdargs[1].Replace("\\","/");
                            var xxx = pluginManager.Load(pathFile);
                           
                             currentPlugin = pluginManager.GetCurrentPlugin(pathFile);

                            currentPlugin.GetInfo();

                            break;
                        default:
                            currentPlugin.GetInfo();
                            break;
                    }
                }
            } while (temp.Equals("exit") == false);

            pluginManager.Dispose();
        }
    }
}