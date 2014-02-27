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
        public double DiseasePeriod 
        {
            get
            {
                return 1 / RecoveryRate;
            }
            set
            {
                RecoveryRate = 1/value;
            }
        }
        public double DiseasePeriodInHours
        {
            get
            {
                return DiseasePeriod * Hours;
            }
            set
            {
                DiseasePeriod = value / Hours;
            }
        }
        public const int Hours = 24;
    }
}
