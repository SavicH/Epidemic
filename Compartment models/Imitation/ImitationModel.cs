using Pigeon.Actor;
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
        private IList<State> state = new List<State>();
        private IList<ActorRef> agents = new List<ActorRef>();
        private System.Threading.ManualResetEvent run = new System.Threading.ManualResetEvent(false);

        private int timeInHours;
        private bool isOver = false;

        public ImitationModel(State initialState, Parameters parameters, double time)
        {
            this.timeInHours = (int)(time*Parameters.Hours+1);
            SimpleAgent.AgentsState = new State();
            var system = new ActorSystem(null);
            ActorRef location = system.ActorOf(Props.Create(() => new SimpleLocation(agents)));

            for (int i = 0; i < initialState.Susceptible; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location, Compartment.Susceptible, parameters))));
            }
            for (int i = 0; i < initialState.Infected; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location, Compartment.Infected, parameters))));
            }
            for (int i = 0; i < initialState.Removed; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(location, Compartment.Removed, parameters))));
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
            foreach (var agent in agents)
            {
                agent.Tell(Messages.Time);
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
                agent.Stop();
            }
            return state;
        }

    }
}
