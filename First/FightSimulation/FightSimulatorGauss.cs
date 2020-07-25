using System;
using System.Collections.Generic;
using Main;

namespace FightSim
{
    //Fight simulator with randomness simulated via normal distibution
    public class FightSimulatorGauss : IFightSimulator
    {

        public FightSimulatorGauss()
        {
        }

        public FightOutcome SimulateFight(Fight fight)
        {
            return new FightState(fight).SimulateFight();
        }

        public (FightOutcome outcome, List<FightStats> Stats) SimulateFightWithDetails(Fight fight)
        {
            FightState liveFight = new FightState(fight);
            liveFight.SimulateFight();

            return (liveFight.Outcome, liveFight.FightStats);
        }
    }
}
