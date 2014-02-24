using Pigeon.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CompartmentModels.Imitation
{
    public class ImitationModel : ICompartmentModel
    {
        private Timer timer = new Timer();
        private IList<State> state = new List<State>();
        private IList<ActorRef> agents = new List<ActorRef>();

        public ImitationModel()
        {
            SimpleAgent.AgentsState = new State();
            var system = new ActorSystem(null);
            ActorRef location = system.ActorOf(Props.Create(() => new SimpleLocation(agents)));

            for (int i = 0; i < 100; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location, Compartment.Susceptible))));
            }
            agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location, Compartment.Infected))));
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 0.1;
            timer.Start();
        }

        int count = 0;

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            State tmp = SimpleAgent.AgentsState;
            state.Add(tmp);
            tmp.Time += 0.04;
            SimpleAgent.AgentsState = tmp;
            foreach (var agent in agents)
            {
                agent.Tell(Messages.Time);
            }
            count++;  // tmp

        }

        public IList<State> Run()
        {
            System.Threading.Thread.Sleep(6000);
            timer.Stop();
            foreach (var agent in agents)
            {
                agent.Stop();
            }
            return state;
        }

    }
}
