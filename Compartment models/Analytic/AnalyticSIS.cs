using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Analytic
{
    abstract class AnalyticSIS : AnalyticModel
    {
        public AnalyticSIS(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 2;
        }
    }
}
