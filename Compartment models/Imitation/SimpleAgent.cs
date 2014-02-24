using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pigeon.Actor;

namespace CompartmentModels.Imitation
{
    class SimpleAgent: UntypedActor
    {
        private static Parameters infectionParameters;
        public static Parameters InfectionParameters
        {
            set
            { 
                infectionParameters = value;
            }
        }

        private int currentTimeDisease;
        private int diseasePeriod = 50; // TMP
        private ActorRef currentLocation;
        
        public Compartment Compartment { get; private set; }

        private static State agentsState = new State();
        public static State AgentsState { get { return agentsState; } set {agentsState = value;} }

        private static Random rand = new Random(DateTime.Now.Millisecond);

        public SimpleAgent(ActorRef currentLocation, Compartment compartment)
        {
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
           return rand.Next() % 3000 == 0;
        }

        private bool IsRemoved()
        {
            return rand.Next() % 5 == 0;
        }

        private void Susceptible(object message)
        {
            if (message == Messages.Infection)
            {
                if (IsInfected())
                {
                    Become(Infected);
                    agentsState.Infected++;
                    agentsState.Susceptible--;
                }
            }

        }

        private void Infected(object message)
        {           
            if (message == Messages.Time)
            {
                if (currentTimeDisease++ == diseasePeriod)
                {
                    Become(Removed);
                    agentsState.Removed++;
                    agentsState.Infected--;
                }
                currentLocation.Tell(Messages.Infection);
            }          
        }

        private void Removed(object message)
        {
        
        }


        protected override void OnReceive(object message)
        {

        }
    }
}
