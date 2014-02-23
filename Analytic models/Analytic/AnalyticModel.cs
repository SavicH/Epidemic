using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompartmentModels.Analytic
{
    public abstract class AnalyticModel : IModel
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
            currentState = initialState;
            this.parameters = parameters;
            this.time = time;
            this.timestep = timestep;
        }

        public abstract IList<State> Run();
    }
}
