using System;
namespace Fighting
{
    public interface FightSimulator
    {
        public FightOutcome SimulateFight(Main.Fight fight);

        public FightState SimulateFightDetailed(Main.Fight fight)
        {
            Console.WriteLine("Optional Method!");
            return null;
        }
    }
}
