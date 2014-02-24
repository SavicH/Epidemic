using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Analytic
{
    class StochasticSIRS : AnalyticSIR
    {
        private Random rand = new Random(DateTime.Now.Millisecond);

        public StochasticSIRS(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
        }

        protected override double[,] CreateDoubleArray()
        {
            int rowCount = (int)Math.Round((time / timestep) + 1);
            double[,] result = new double[rowCount, 4];
            double currentTime = 0;

            for (int i = 0; i < rowCount; i++)
            {
                result[i, 0] = currentTime;
                result[i, 1] = currentState.Susceptible;
                result[i, 2] = currentState.Infected;
                result[i, 3] = currentState.Removed;

                double[] prob = Probabilities(currentState);
                currentTime += timestep;

                int which = process(prob);
                switch (which)
                {
                    case 0: currentState.Susceptible--; currentState.Infected++; break;
                    case 1: currentState.Infected--; currentState.Removed++; break;
                    case 2: currentState.Susceptible++; currentState.Infected--; break;
                    case 3: currentState.Susceptible++; currentState.Removed--; break;
                    case 4: break;
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
            double[] a = new double[5];

            a[0] = parameters.InfectionRate * state.Susceptible * state.Infected/ parameters.Population * timestep; // вероятность заболевания
            a[1] = parameters.RecoveryRate * state.Infected * timestep; // вероятность выздоровления
            a[2] = parameters.BirthRate * state.Infected * timestep; // рождение восприимчивого, смерть инфицированного
            a[3] = (parameters.BirthRate + parameters.SusceptibleRate)* (parameters.Population - state.Susceptible - state.Infected) * timestep; // вероятность рождения восприимчивого, смерть выздоровевшего
            a[4] = 1 - a[0] - a[1] - a[2] - a[3]; // вероятность того, что ничего не произойдет
            return a;
        }

    }
}
