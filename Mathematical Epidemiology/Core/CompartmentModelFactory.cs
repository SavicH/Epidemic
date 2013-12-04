using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathematicalEpidemiology.Core
{
    static class CompartmentModelFactory
    {
        public static CompartmentModel CreateModel(CompartmentModelType type, bool isStochastic, State initialState,
            Parameters parameters, double time, double timestep)
        {
            CompartmentModel model = null;
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
                        break;
                    else
                        model = new DeterministicSEIR(initialState, parameters, time, timestep);
                    break;
            }
            return model;
        }
    }
}
