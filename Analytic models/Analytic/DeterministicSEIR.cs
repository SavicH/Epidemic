using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNumerics.ODE;

namespace CompartmentModels.Analytic
{
    class DeterministicSEIR : AnalyticModel
    {
        private OdeExplicitRungeKutta45 ode = new OdeExplicitRungeKutta45();

        public DeterministicSEIR(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 4;
        }

        public override double[,] Run()
        {
            OdeFunction function = new OdeFunction(ODEs);
            ode.InitializeODEs(function, compartmentsCount);
            double[,] solution = ode.Solve(new Double[] { currentState.Susceptible, currentState.Infected, currentState.Exposed, currentState.Removed },
                0, timestep, time);
            return solution;
        }

        private double[] ODEs(double t, double[] y)
        {
            double S = y[0];
            double I = y[1];
            double E = y[2];         
            double R = y[3];

            double[] SEIR = new double[compartmentsCount];

            SEIR[0] = parameters.BirthRate - parameters.InfectionRate * S * I - parameters.BirthRate * S;       
            SEIR[1] = parameters.ExposedRate * E - parameters.RecoveryRate * I - parameters.BirthRate * I;
            SEIR[2] = parameters.InfectionRate * S * I - (parameters.BirthRate + parameters.ExposedRate) * E;
            SEIR[3] = parameters.RecoveryRate * I - parameters.BirthRate * R;                                    
            return SEIR;
        }
    }
}
