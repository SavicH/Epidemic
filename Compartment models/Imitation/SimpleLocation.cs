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

        protected override void OnReceive(object message)
        {
            if (message == Messages.Infection)
            {
                foreach (var agent in agents)
                {
                    agent.Tell(Messages.Infection);
                }
            }
        }

        public SimpleLocation(IList<ActorRef> agents)
        {
            this.agents = agents;
        }
    }
}
