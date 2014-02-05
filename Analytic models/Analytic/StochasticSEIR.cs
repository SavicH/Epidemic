using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Analytic
{
    class StochasticSEIR: AnalyticSEIR
    {
        private Random rand = new Random(DateTime.Now.Millisecond);

        public StochasticSEIR(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
        }

        protected override double[,] CreateDoubleArray()
        {
            int rowCount = (int)Math.Round((time / timestep) + 1);

            double[,] result = new double[rowCount, 5];
            double currentTime = 0;

            for (int i = 0; i < rowCount; i++)
            {
                result[i, 0] = currentTime;
                result[i, 1] = currentState.Susceptible;
                result[i, 2] = currentState.Infected;
                result[i, 3] = currentState.Removed;
                result[i, 4] = currentState.Exposed;

                double[] prob = Probabilities(currentState);
                currentTime += timestep;

                int which = process(prob);
                switch (which)
                {
                    case 0: currentState.Susceptible--; currentState.Exposed++; break;
                    case 1: currentState.Infected++; currentState.Exposed--; break;
                    case 2: currentState.Infected--; currentState.Removed++; break;
                    case 3: currentState.Susceptible++; currentState.Exposed--; break;
                    case 4: currentState.Infected--; currentState.Susceptible++; break;
                    case 5: currentState.Removed++; currentState.Susceptible++; break;
                }

            }

            return result;

        }

        int process(double[] probs)
        {
            double r = rand.NextDouble();
            int i = -1;
            double offset = 0;
            do
            {
                offset += probs[++i];
            }
            while (r > offset);
            return i;
        }

        double[] Probabilities(State state)
        {
            double[] a = new double[7];

            a[0] = parameters.InfectionRate * state.Susceptible * state.Infected / parameters.Population * timestep;
            a[1] = parameters.ExposedRate * state.Exposed * timestep;
            a[2] = parameters.RecoveryRate * state.Infected * timestep; 
            a[3] = parameters.BirthRate * state.Exposed * timestep;
            a[4] = parameters.BirthRate * state.Infected * timestep;                  
            a[5] = parameters.BirthRate * (parameters.Population - state.Susceptible - state.Infected - state.Exposed) * timestep; // вероятность рождения восприимчивого, смерть выздоровевшего
            a[6] = 1 - a[0] - a[1] - a[2] - a[3] - a[4] - a[5]; // вероятность того, что ничего не произойдет
            return a;
        }
    }
}
