using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class DateTimeUtility
    {
        public static DateTime ToDateTime(this long input)
        {
            DateTime start = new DateTime(1970, 1, 1);
            DateTime date = start.AddSeconds(input);
            return date;
        }
    }
}
