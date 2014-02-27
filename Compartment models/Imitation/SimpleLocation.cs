using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pigeon.Actor;

namespace CompartmentModels.Imitation
{
    class SimpleLocation : UntypedActor
    {
        private IList<ActorRef> agents;
        private double rate;

        protected override void OnReceive(object message)
        {
            lock (agents)
            {
                if (message is Infection)
                {
                    foreach (var agent in agents)
                    {
                        agent.Tell(new Infection(rate));
                    }
                }

                if (message == Messages.LeaveLocation)
                {
                    agents.Remove(Sender);
                }
                if (message == Messages.EnterLocation)
                {
                    agents.Add(Sender);
                }
            }
        }

        public SimpleLocation(IList<ActorRef> agents, double rate)
        {
            this.agents = agents;
            this.rate = rate;
        }
        
    }
}
