using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Logging
{
    public interface ILogger
    {
        void Log(MessageLevel level, string message, params object[] args);
    }
}
