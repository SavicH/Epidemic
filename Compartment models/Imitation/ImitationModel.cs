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
        private ActorSystem system;
        private Timer timer = new Timer();
        private State initialState;
        private Parameters parameters;
        private IList<State> state;
        private IList<ActorRef> agents;
        private IList<ActorRef> locations;
        private System.Threading.ManualResetEvent run = new System.Threading.ManualResetEvent(false);

        private int timeInHours;
        private bool isOver = false;

        public ImitationModel(State initialState, Parameters parameters, double time)
        {
            this.timeInHours = (int)(time * Parameters.Hours + 1);
            this.initialState = initialState;
            this.parameters = parameters;
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 20;
            timer.Stop();
        }

        int count = 0;

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            State tmp = SimpleAgent.AgentsState;
            state.Add(tmp);
            tmp.Time += 1.0 / Parameters.Hours;
            SimpleAgent.AgentsState = tmp;
                foreach (var agent in agents.AsEnumerable())
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
            StartModel();
            timer.Start();
            while (!isOver)    // very tmp
            { }
            run.Reset();  // doesn't work
            timer.Stop();
            StopModel();
            return state;
        }

        private void StopModel()
        {
            foreach (var agent in agents.AsEnumerable())
            {
                system.Stop(agent);
            }
            foreach (var location in locations.AsEnumerable())
            {
                system.Stop(location);
            }
        }

        private void StartModel()
        {
            count = 0;
            isOver = false;
            state = new List<State>();
            agents = new List<ActorRef>();
            locations = new List<ActorRef>();
            SimpleAgent.AgentsState = new State();
            system = new ActorSystem(null);
            locations.Add(system.ActorOf(Props.Create(() => new SimpleLocation(agents, 1))));
            locations.Add(system.ActorOf(Props.Create(() => new SimpleLocation(agents, 2))));
            Dictionary<int, ActorRef> tmpSchedule = new Dictionary<int, ActorRef>() { { 0, locations[0] }, { 20, locations[1] } };
            for (int i = 0; i < initialState.Susceptible; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(locations[0], Compartment.Susceptible, parameters, tmpSchedule))));
            }
            for (int i = 0; i < initialState.Infected; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(locations[0], Compartment.Infected, parameters, tmpSchedule))));
            }
            for (int i = 0; i < initialState.Removed; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(locations[0], Compartment.Removed, parameters, tmpSchedule))));
            }
        }

    }
}
