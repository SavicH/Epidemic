using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Analytic
{
    class StochasticSIRS : StochasticModel
    {
        public StochasticSIRS(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 3;
        }

        protected override void ChangeState(int which)
        {
            switch (which)
            {
                case 0: currentState.Susceptible--; currentState.Infected++; break;
                case 1: currentState.Infected--; currentState.Removed++; break;
                case 2: currentState.Susceptible++; currentState.Infected--; break;
                case 3: currentState.Susceptible++; currentState.Removed--; break;
                case 4: break;
            }
        }
        
        protected override double[] Probabilities(State state)
        {
            double[] a = new double[5];

            a[0] = parameters.InfectionRate * state.Susceptible * state.Infected / parameters.Population * timestep; // вероятность заболевания
            a[1] = parameters.RecoveryRate * state.Infected * timestep; // вероятность выздоровления
            a[2] = parameters.BirthRate * state.Infected * timestep; // рождение восприимчивого, смерть инфицированного
            a[3] = (parameters.BirthRate + parameters.SusceptibleRate) * (parameters.Population - state.Susceptible - state.Infected) * timestep; // вероятность рождения восприимчивого, смерть выздоровевшего
            a[4] = 1 - a[0] - a[1] - a[2] - a[3]; // вероятность того, что ничего не произойдет
            return a;
        }

    }
}
