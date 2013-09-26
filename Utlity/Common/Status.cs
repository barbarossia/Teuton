using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utlity.Common
{
    public enum Status
    {
        Ready,
        Converting,
        Converted,
        ConvertedFail,
        Sending,
        SendCanceled,
        SendFailed,
        Done
    }
}
