using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CompartmentModels.Imitation
{
    public class ImitationModel : IModel
    {
       
        private Timer timer = new Timer();
        private ActorSystem system;
        private IList<State> state = new List<State>();
        private IList<ActorRef> agents = new List<ActorRef>();
        private IList<ActorRef> locations = new List<ActorRef>();
        private System.Threading.ManualResetEvent run = new System.Threading.ManualResetEvent(false);

        private int timeInHours;
        private bool isOver = false;

        public ImitationModel(State initialState, Parameters parameters, double time)
        {
            system = new ActorSystem("city");
            this.timeInHours = (int)(time*Parameters.Hours+1);
            SimpleAgent.AgentsState = new State();
            ActorRef location1 = system.ActorOf(Props.Create(() => new SimpleLocation(agents, 1)));
            ActorRef location2 = system.ActorOf(Props.Create(() => new SimpleLocation(agents, 2)));

            Dictionary<int, ActorRef> tmpSchedule = new Dictionary<int, ActorRef>(){ {0, location1}, {20, location2}};
            for (int i = 0; i < initialState.Susceptible; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location1, Compartment.Susceptible, parameters, tmpSchedule))));
            }
            for (int i = 0; i < initialState.Infected; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location1, Compartment.Infected, parameters, tmpSchedule))));
            }
            for (int i = 0; i < initialState.Removed; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location1, Compartment.Removed, parameters, tmpSchedule))));
            }

            timer.Elapsed += timer_Elapsed;
            timer.Interval = 20;
            timer.Stop();
        }

        int count = 0;

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            State tmp = SimpleAgent.AgentsState;
            state.Add(tmp);
            tmp.Time += 1.0/Parameters.Hours;
            SimpleAgent.AgentsState = tmp;
            lock (agents)
            {
                foreach (var agent in agents)
                {
                    agent.Tell(Messages.Time);
                }
            }
            count++;  // tmp
            if (count == timeInHours)
            {
                isOver = true;
                run.Set();
            }

        }

        public IList<State> Run()
        {
            timer.Start();
            while (!isOver)    // very tmp
            { }
            run.Reset();  // doesn't work
            timer.Stop();
            foreach (var agent in agents)
            {
                system.Stop(agent);
            }
            return state;
        }

    }
}
