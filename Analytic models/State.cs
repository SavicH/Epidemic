using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels
{
    public struct State
    {
        public double Time { get; set; }

        public double Susceptible { get; set; }
        public double Infected { get; set; }
        public double Removed { get; set; }
        public double Exposed { get; set; }

        public double Population { get { return Susceptible + Infected + Removed + Exposed; } }
    }
}
