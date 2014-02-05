using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels
{
    interface ICompartmentModel
    {
        IList<State> Run();   
    }
}
