using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels.Analytic
{
    public abstract class AnalyticModel : ICompartmentModel
    {
        protected State currentState;
        protected Parameters parameters;

        protected int compartmentsCount;

        protected double time;
        protected double timestep;

        public int CompartmentCount
        {
            get 
            {
                return compartmentsCount;
            }
        }

        public AnalyticModel(State initialState, Parameters parameters, double time, double timestep)
        {
            if (!CheckState(initialState, parameters.Population))
            {
                throw new AnalyticModelException();
            }
            currentState = initialState;
            this.parameters = parameters;
            this.time = time;
            this.timestep = timestep;
        }

        protected static bool CheckState(State state, double population)
        {
            double eps = 10e-6;
            return Math.Abs(state.Population - population) < eps;
        }

        protected abstract double[,] CreateDoubleArray();

        protected virtual void FillState(ref State state, double[,] solution, int row) {
            state.Time = solution[row, 0];
            state.Infected = solution[row, 2];
            state.Susceptible = solution[row, 1];
        }

        public IList<State> Run()
        {
            double[,] solution = CreateDoubleArray();
            int n = solution.Length / (compartmentsCount + 1);
            List<State> result = new List<State>(n);
            for (int i = 0; i < result.Capacity; i++)
            {
                State tmp = new State();
                FillState(ref tmp, solution, i);
                result.Add(tmp);
            }
            return result;
        }
    }
}
