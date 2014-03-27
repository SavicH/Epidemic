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
        private List<State> state;
        private List<ActorRef> agents;
        private Dictionary<string, ActorRef> locations;
        private System.Threading.ManualResetEvent run = new System.Threading.ManualResetEvent(false);

        private int timeInHours;
        private bool isOver = false;

        public ImitationModel(State initialState, Parameters parameters, double time)
        {
            this.timeInHours = (int)(time * Parameters.Hours + 1);
            this.initialState = initialState;
            this.parameters = parameters;
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 300;
            timer.Stop();
        }

        int count = 0;

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            State tmp = SimpleAgent.AgentsState;
            state.Add(tmp);
            tmp.Time += 1.0 / Parameters.Hours;
            SimpleAgent.AgentsState = tmp;
            for (int i = 0; i < agents.Count && agents[i] != null; i++ )
            {
                agents[i].Tell(new Time());
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
            //StopModel();
            return state;
        }

        public void CreateLocations()
        {
            List<ActorRef> agentsCopy = new List<ActorRef>();
            agentsCopy.AddRange(agents);
            locations = new Dictionary<string, ActorRef>();
            locations.Add("school", system.ActorOf(Props.Create(() => new SimpleLocation(new List<ActorRef>(), 5))));
            locations.Add("home", system.ActorOf(Props.Create(() => new SimpleLocation(agentsCopy, 1))));
        }

        private void StopModel()
        {
            foreach (var agent in agents.AsReadOnly())
            {
                system.Stop(agent);
            }
            foreach (var location in locations)
            {
                system.Stop(location.Value);
            }
        }

        private void StartModel()
        {
            count = 0;
            isOver = false;
            state = new List<State>();
            agents = new List<ActorRef>();
           
            SimpleAgent.AgentsState = new State();
            system = new ActorSystem(null);

            CreateLocations();
            
            Dictionary<int, ActorRef> tmpSchedule = new Dictionary<int, ActorRef>() { { 8, locations["school"] }, { 18, locations["home"] } };
            for (int i = 0; i < initialState.Susceptible; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(locations["home"], Compartment.Susceptible, parameters, tmpSchedule))));
            }
            for (int i = 0; i < initialState.Infected; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(locations["home"], Compartment.Infected, parameters, tmpSchedule))));
            }
            for (int i = 0; i < initialState.Removed; i++)
            {
                agents.Add(system.ActorOf(Props.Create(() => new SimpleAgent(locations["home"], Compartment.Removed, parameters, tmpSchedule))));
            }
        }

    }
}
