using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Analytic
{
    abstract class StochasticModel : AnalyticModel
    {
        private Random random = new Random(DateTime.Now.Millisecond);

        public StochasticModel(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
        }

        public override IList<State> Run()
        {
            int count = (int)Math.Round((time / timestep) + 1);
            IList<State> result = new List<State>(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(currentState);
                currentState.Time += timestep;

                double[] prob = Probabilities(currentState);              
                int which = process(prob);
                ChangeState(which);

            }
            return result;
        }

        protected int process(double[] probs)
        {
            double r = random.NextDouble();
            int i = -1;
            double offset = 0;
            do
            {
                offset += probs[++i];
            }
            while (r > offset);
            return i;
        }

        protected abstract void ChangeState(int which);

        protected abstract double[] Probabilities(State state);
    }
}
