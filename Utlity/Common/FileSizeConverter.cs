using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Common
{
    /// <summary>
    /// Convert any of the standard suffixes (such as TB, MB, GB) to single size
    /// reference: http://rextester.com/rundotnet?code=BQYCB2587
    /// </summary>
    public class FileSizeConverter
    {
        private static System.Globalization.NumberFormatInfo numberFormat;
        private static Dictionary<string, long> knownUnits;

        static FileSizeConverter()
        {
            knownUnits = new Dictionary<string, long>
            { 
                { "", 1L },                                 // no unit is same as unit B(yte)
                { "B", 1L },
                { "KB", 1024L },
                { "K", 1024L },
                { "MB", 1024L * 1024L},
                { "M", 1024L * 1024L},
                { "GB", 1024L * 1024L * 1024L},
                { "G", 1024L * 1024L * 1024L},
                { "TB", 1024L * 1024L * 1024L * 1024L},
                { "T", 1024L * 1024L * 1024L * 1024L}
                // fill rest as needed
            };

            // since I live in a locale where "," is the decimal separator I will enforce US number format
            numberFormat = new System.Globalization.CultureInfo("en-US").NumberFormat;
        }

        public long Parse(string value)
        {
            // ignore spaces around the actual value
            value = value.Trim();

            string unit = ExtractUnit(value);
            string sizeAsString = value.Substring(0, value.Length - unit.Length).Trim();  // trim spaces

            long multiplicator = MultiplicatorForUnit(unit);
            decimal size;

            if (!decimal.TryParse(sizeAsString, System.Globalization.NumberStyles.Number, numberFormat, out size))
                throw new ArgumentException("illegal number", "value");

            return (long)(multiplicator * size);
        }

        private string ExtractUnit(string sizeWithUnit)
        {
            // start right, end at the first digit
            int lastChar = sizeWithUnit.Length - 1;
            int unitLength = 0;
            char[] stringArray = sizeWithUnit.ToCharArray();

            while (unitLength <= lastChar
                && stringArray[lastChar - unitLength] != ' '       // stop when a space
                && !Char.IsDigit(stringArray[lastChar - unitLength]))   // or digit is found
            {
                unitLength++;
            }

            return sizeWithUnit.Substring(sizeWithUnit.Length - unitLength).ToUpperInvariant();
        }

        private long MultiplicatorForUnit(string unit)
        {
            unit = unit.ToUpperInvariant();

            if (!knownUnits.ContainsKey(unit))
                throw new ArgumentException("illegal or unknown unit", "unit");

            return knownUnits[unit];
        }
    }
}
