using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CompartmentModels.Analytic
{
    class AnalyticModelException : Exception
    {
        public AnalyticModelException()
            :base()
        {

        }
        
        public AnalyticModelException(string message)
            : base(message)
        {

        }
        
        protected AnalyticModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public AnalyticModelException(string message, Exception innerException)
            :base(message, innerException)
        {

        }
    }
}
