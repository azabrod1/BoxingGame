using System;
using System.Collections.Generic;

namespace FightSim
{
    public interface FightSimulator
    {
        public FightOutcome SimulateFight(Main.Fight fight);

        //round by round summary
        public (FightOutcome outcome, List<FightStats> Stats) SimulateFightWithDetails(Main.Fight fight)
        {
            //Optional Method!
            throw new NotImplementedException();
        }
    }
}
