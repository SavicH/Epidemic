using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels.Analytic
{
    class StochasticSIS : AnalyticModel
    {
        private Random rand = new Random(DateTime.Now.Millisecond);

        public StochasticSIS(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 2;
        }

        public override double[,] Run()
        {
            int rowCount = (int)Math.Round((time / timestep) + 1);
            double[,] result = new double[rowCount, 3];
            double currentTime = 0;

            for (int i = 0; i < rowCount; i++)
            {
                result[i, 0] = currentTime;
                result[i, 1] = currentState.Susceptible;
                result[i, 2] = currentState.Infected;

                double[] prob = Probabilities(currentState);
                currentTime += timestep;

                int which = process(prob);
                switch (which)
                {
                    case 0: currentState.Susceptible--; currentState.Infected++; break;
                    case 1: currentState.Infected--; currentState.Susceptible++; break;
                    case 2: break;
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
            double[] a = new double[3];

            a[0] = parameters.InfectionRate * state.Susceptible * state.Infected / parameters.Population * timestep; // вероятность заболевания
            a[1] = (parameters.BirthRate + parameters.RecoveryRate) * state.Infected * timestep; // вероятность выздоровления
            a[2] = 1 - a[0] - a[1]; // вероятность того, что ничего не произойдет
            return a;
        }

    }
}
