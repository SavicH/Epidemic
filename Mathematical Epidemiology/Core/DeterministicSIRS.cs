using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNumerics.ODE;

namespace MathematicalEpidemiology.Core
{
    class DeterministicSIRS: CompartmentModel
    {
         private OdeExplicitRungeKutta45 ode = new OdeExplicitRungeKutta45();

        public DeterministicSIRS(State initialState, Parameters parameters, double time, double timestep)
            :base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 3;
        }

        public override double[,] Run()
        {
            OdeFunction function = new OdeFunction(ODEs);
            ode.InitializeODEs(function, compartmentsCount);
            double[,] solution = ode.Solve(new Double[] {currentState.Susceptible, currentState.Infected, currentState.Removed},
                0, timestep, time);
            return solution;
        }

        private double[] ODEs(double t, double[] y)
        {
            double S = y[0];
            double I = y[1];
            double R = y[2];      
            double[] SIRS = new double[compartmentsCount];

            SIRS[0] = parameters.BirthRate - parameters.InfectionRate * S * I - parameters.BirthRate * S + parameters.SusceptibleRate*R;         // dS/dt = −βSI + b - b*S + fR;
            SIRS[1] = parameters.InfectionRate * S * I - parameters.RecoveryRate * I - parameters.BirthRate * S;  // dI/dt = βSI − γI - b*I;
            SIRS[2] = parameters.RecoveryRate * I - parameters.BirthRate * R - parameters.SusceptibleRate*R;                                     // dR/dt = γI - b*R - fR; 
            return SIRS;
        }
    }
}
