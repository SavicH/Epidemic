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

        public static State operator +(State a, State b) 
        {
            State c = new State();
            c.Time = a.Time;
            c.Susceptible = a.Susceptible + b.Susceptible;
            c.Infected = a.Infected + b.Infected;
            c.Removed = a.Removed + b.Removed;
            c.Exposed = a.Exposed + b.Exposed;
            return c;
        }

        public static State operator /(State s, int x)
        {
            State c = new State();
            c.Time = s.Time;
            c.Susceptible = s.Susceptible / x;
            c.Infected = s.Infected / x;
            c.Removed = s.Removed / x;
            c.Exposed = s.Exposed / x;
            return c;
        }
    }
}
