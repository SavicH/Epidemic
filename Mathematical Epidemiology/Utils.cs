using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEpidemiology
{
    static class Utils
    {
        public static double ParseDoubleInvariantly(string s)
        {
            return double.Parse(
                s.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),
                CultureInfo.InvariantCulture);
        }
    }
}
