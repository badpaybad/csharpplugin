using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicPlugin.Core
{
    public interface IPlugin:IDisposable
    {
        void GetInfo();
    }
}
