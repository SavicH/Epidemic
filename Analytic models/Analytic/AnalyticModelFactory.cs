using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels.Analytic
{
    public static class AnalyticModelFactory
    {
        public static AnalyticModel CreateModel(CompartmentModelType type, bool isStochastic, State initialState,
            Parameters parameters, double time, double timestep)
        {
            AnalyticModel model = null;
            switch (type)
            {
                case CompartmentModelType.SIR:
                    if (isStochastic)
                        model = new StochasticSIR(initialState, parameters, time, timestep);
                    else
                        model = new DeterministicSIR(initialState, parameters, time, timestep);
                    break;
                case CompartmentModelType.SIS:
                    if (isStochastic)
                        model = new StochasticSIS(initialState, parameters, time, timestep);
                    else
                        model = new DeterministicSIS(initialState, parameters, time, timestep);
                    break;
                case CompartmentModelType.SIRS:
                    if (isStochastic)
                        model = new StochasticSIRS(initialState, parameters, time, timestep);
                    else
                        model = new DeterministicSIRS(initialState, parameters, time, timestep);
                    break;
                case CompartmentModelType.SEIR:
                    if (isStochastic)
                        model = new StochasticSEIR(initialState, parameters, time, timestep);
                    else
                        model = new DeterministicSEIR(initialState, parameters, time, timestep);
                    break;
            }
            return model;
        }
    }
}
