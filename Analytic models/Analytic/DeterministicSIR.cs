using DotNumerics.ODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels.Analytic
{
    class DeterministicSIR : DeterministicModel
    {
        private OdeExplicitRungeKutta45 ode = new OdeExplicitRungeKutta45();

        public DeterministicSIR(State initialState, Parameters parameters, double time, double timestep)
            :base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 3;
            initialConditions = new double[] { currentState.Susceptible, currentState.Infected, currentState.Removed };
        }

        protected override void FillState(ref State state, double[,] solution, int row)
        {
            base.FillState(ref state, solution, row);
            state.Removed = solution[row, 3];
        }

        protected override double[] ODEs(double t, double[] y)
        {
            double S = y[0];
            double I = y[1];
            double R = y[2];
            double[] SIR = new double[compartmentsCount];

            SIR[0] = parameters.BirthRate - parameters.InfectionRate * S * I - parameters.BirthRate * S;         // dS/dt = −βSI + b - b*S;
            SIR[1] = parameters.InfectionRate * S * I - parameters.RecoveryRate * I - parameters.BirthRate * S;  // dI/dt = βSI − γI - b*I;
            SIR[2] = parameters.RecoveryRate * I - parameters.BirthRate * R;                                     // dR/dt = γI - b*R; 
            return SIR;
        }
    }
}
