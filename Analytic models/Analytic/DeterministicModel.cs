using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNumerics.ODE;

namespace CompartmentModels.Analytic
{
    abstract class DeterministicModel : AnalyticModel
    {
        private OdeExplicitRungeKutta45 ode = new OdeExplicitRungeKutta45();
        protected double[] initialConditions;

        public DeterministicModel(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            
        }

        protected virtual double[,] SolveEquations()
        {
            OdeFunction function = new OdeFunction(ODEs);
            ode.InitializeODEs(function, compartmentsCount);
            double[,] solution = ode.Solve(initialConditions,
                0, timestep, time);
            return solution;
        }

        protected abstract double[] ODEs(double t, double[] y);

        protected virtual void FillState(ref State state, double[,] solution, int row)
        {
            state.Time = solution[row, 0];
            state.Infected = solution[row, 2];
            state.Susceptible = solution[row, 1];
        }

        public override IList<State> Run()
        {
            double[,] solution = SolveEquations();
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
