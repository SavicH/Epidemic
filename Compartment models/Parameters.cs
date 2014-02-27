using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels
{
    public struct Parameters
    {
        public double Population { get; set; }
        public double RecoveryRate { get; set; }
        public double InfectionRate { get; set; }
        public double BirthRate { get; set; }
        public double SusceptibleRate { get; set; }
        public double ExposedRate { get; set; }
    }
}
