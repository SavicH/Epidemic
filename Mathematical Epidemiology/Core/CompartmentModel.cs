using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathematicalEpidemiology.Core
{
    public abstract class CompartmentModel
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

        public CompartmentModel(State initialState, Parameters parameters, double time, double timestep)
        {
            if (!CheckState(initialState, parameters.Population))
            {
                throw new CompartmentModelException();
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

        public abstract double[,] Run();
    }
}
