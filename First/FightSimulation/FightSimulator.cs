using System;
namespace FightSim
{
    public interface FightSimulator
    {
        public FightOutcome SimulateFight(Main.Fight fight);

        public FightState SimulateFightDetailed(Main.Fight fight)
        {
            //Optional Method!
            throw new NotImplementedException();
        }
    }
}
