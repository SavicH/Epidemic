using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;

namespace CompartmentModels.Imitation
{
    class SimpleLocation : UntypedActor
    {
        private List<ActorRef> agents;
        private double rate;

        protected override void OnReceive(object message)
        {
            if (message is Infection)
            {
                //lock (agents) {
                    for (int i = 0; i < agents.Count; i++)
                    {
                        agents[i].Tell(new Infection(rate));
                    }
               // }
            }

            if (message is LeaveLocation)
            {
                agents.Remove(Sender);
            }
            if (message is EnterLocation)
            {
                agents.Add(Sender);
            }
        }

        public SimpleLocation(List<ActorRef> agents, double rate)
        {
            this.agents = agents;
            this.rate = rate;
        }

    }
}
