using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNumerics.ODE;

namespace CompartmentModels.Analytic
{
    class DeterministicSEIR : DeterministicModel
    {
        public DeterministicSEIR(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 4;
            initialConditions = new double[] { currentState.Susceptible, currentState.Infected, currentState.Exposed, currentState.Removed };
        }

        protected override void FillState(ref State state, double[,] solution, int row)
        {
            base.FillState(ref state, solution, row);
            state.Exposed = solution[row, 4];
        }
            
        protected override double[] ODEs(double t, double[] y)
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
