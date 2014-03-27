using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Akka.Actor;

namespace CompartmentModels.Imitation
{
    class SimpleAgent: UntypedActor
    {
        private Dictionary<int, ActorRef>  schedule;
        private const double rate = 500; // tmp value
        private double locationRate = 1;
        private static Parameters infectionParameters;
        public static Parameters InfectionParameters
        {
            set
            { 
                infectionParameters = value;
            }
        }

        private int currentTimeDisease;
        public static Parameters parameters; // TMP
       
        private ActorRef currentLocation;
        private int currentTime;
        
        public Compartment Compartment { get; private set; }

        private static State agentsState = new State();
        public static State AgentsState { get { return agentsState; } set {agentsState = value;} }

        private static Random rand = new Random(DateTime.Now.Millisecond);

        public SimpleAgent(ActorRef currentLocation, Compartment compartment, Parameters parameters, Dictionary<int, ActorRef> schedule)
        {
            this.schedule = schedule;
            SimpleAgent.parameters = parameters;
            this.currentLocation = currentLocation;
            
            switch (compartment)
            {
                case Compartment.Susceptible: Become(Susceptible); agentsState.Susceptible++; break;
                case Compartment.Infected: Become(Infected); agentsState.Infected++; break;
                case Compartment.Removed: Become(Removed); agentsState.Removed++;  break;
            }
        }

        private bool IsInfected()
        {
           return rand.Next() % (parameters.InfectionRate*rate*locationRate) == 0;
        }

        private void Susceptible(object message)
        {     
            if (message is Time)
            {
                ChangeLocation();
            }
            if (message is Infection)
            {
                if (IsInfected())
                {
                    locationRate = (message as Infection).Rate;
                    Become(Infected);
                    agentsState.Infected++;
                    agentsState.Susceptible--;
                }
            }

        }

        private void Infected(object message)
        {
            if (message is Time)
            {
                ChangeLocation();
                if (currentTimeDisease++ == parameters.DiseasePeriodInHours)
                {
                    Become(Removed);
                    agentsState.Removed++;
                    agentsState.Infected--;
                }
                currentLocation.Tell(new Infection());
            }          
        }

        private void ChangeLocation()
        {
            currentTime = (++currentTime) % Parameters.Hours;
            if (schedule.ContainsKey(currentTime))
            { 
                currentLocation.Tell( new LeaveLocation());
                currentLocation = schedule[currentTime];
                currentLocation.Tell( new EnterLocation());
                 
            }
          
        }

        private void Removed(object message)
        {
            if (message is Time)
            {
                currentTime = (++currentTime) % Parameters.Hours;
            }
        }


        protected override void OnReceive(object message)
        {

        }
    }
}
