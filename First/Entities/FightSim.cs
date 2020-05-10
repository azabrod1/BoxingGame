using System;
namespace Main
{
    public interface FightSim
    {
        // simulates a fight f, returns f object (modified)
        public Fight simulate(Fight f);

    }
}
