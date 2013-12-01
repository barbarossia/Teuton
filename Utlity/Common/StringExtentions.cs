using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class StringExtentions
    {
        public static bool IsDigit(this string str)
        {
            return str.Length > 0 && str.All(c => Char.IsDigit(c));
        }
    }
}
