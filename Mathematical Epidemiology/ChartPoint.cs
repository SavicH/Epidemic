using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathematicalEpidemiology
{
    class ChartPoint
    {
        public double Value { get; set; }
        public double Time { get; set; }

        public ChartPoint(double time, double value)
        {
            Value = value;
            Time = time;
        }
    }
}
