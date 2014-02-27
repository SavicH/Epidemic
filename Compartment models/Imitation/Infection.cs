using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Imitation
{
    class Infection
    {
        public double Rate{get; private set;}

        public Infection(double rate)
        {
            this.Rate = rate;
        }

        public Infection()
        { }
    }
}
