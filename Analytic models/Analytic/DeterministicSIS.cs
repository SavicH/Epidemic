using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNumerics.ODE;

namespace CompartmentModels.Analytic
{
    class DeterministicSIS : DeterministicModel
    {
        private OdeExplicitRungeKutta45 ode = new OdeExplicitRungeKutta45();

        public DeterministicSIS(State initialState, Parameters parameters, double time, double timestep)
            :base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 2;
            initialConditions = new double[] { currentState.Susceptible, currentState.Infected };
        }

        protected override double[] ODEs(double t, double[] y)
        {
            double S = y[0];
            double I = y[1];
           
            double[] SIS = new double[compartmentsCount];

            SIS[0] =  - parameters.InfectionRate * S * I + parameters.BirthRate - parameters.BirthRate * S;         // dS/dt = −βSI + b - b*S;
            SIS[1] = parameters.InfectionRate * S * I - parameters.RecoveryRate * I - parameters.BirthRate * S;  // dI/dt = βSI − γI - b*I; 
            return SIS;
        }
    }
}
