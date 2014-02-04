using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AnalyticModels
{
    class CompartmentModelException : Exception
    {
        public CompartmentModelException()
            :base()
        {

        }
        
        public CompartmentModelException(string message)
            : base(message)
        {

        }
        
        protected CompartmentModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public CompartmentModelException(string message, Exception innerException)
            :base(message, innerException)
        {

        }
    }
}
