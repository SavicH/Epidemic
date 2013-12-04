using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathematicalEpidemiology.Core
{
    public class Parameters
    {
        private double population = 1;

        public double RecoveryRate { get; set; }
        public double InfectionRate { get; set; }
        public double BirthRate { get; set; }
        public double Population { get { return population; } set { population = value; } }
        public double SusceptibleRate { get; set; }
        public double ExposedRate { get; set; }
    }
}
