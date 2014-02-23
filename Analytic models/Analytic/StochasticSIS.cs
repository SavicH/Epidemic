using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels.Analytic
{
    class StochasticSIS : StochasticModel
    {
        public StochasticSIS(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 2;
        }

        protected override void ChangeState(int which)
        {
            switch (which)
            {
                case 0: currentState.Susceptible--; currentState.Infected++; break;
                case 1: currentState.Infected--; currentState.Susceptible++; break;
                case 2: break;
            }
        }
                
        protected override double[] Probabilities(State state)
        {
            double[] a = new double[3];
            a[0] = parameters.InfectionRate * state.Susceptible * state.Infected / parameters.Population * timestep;
            a[1] = (parameters.BirthRate + parameters.RecoveryRate) * state.Infected * timestep;
            a[2] = 1 - a[0] - a[1];
            return a;
        }

    }
}
