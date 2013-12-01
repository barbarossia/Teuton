using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindleConverter_WPF.Models
{
    public enum BookStatus
    {
        Ready,
        Converting,
        Converted,
        Canceled,
        Failed,
        Sending,
        Completed,
        Error,
    }
}
