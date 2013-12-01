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
                        model = null;
                    else
                        model = new DeterministicSIR(initialState, parameters, time, timestep);
                    break;
            }
            return model;
        }
    }
}
