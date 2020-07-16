using System;
using FightSim;
namespace Main
{
    //Holds perm vars about the fight - eventually we should split it out in fight boxscore( total bunches) and fight info (like venue)
    public class Fight
    {
        public readonly int RoundsScheduled;
        public readonly Fighter[] Fighers;

        public Judge[] Judges;
        public FightOutcome Outcome;
        public DateTime Date;

        public Fight()
        {
        }

        public Fight(Fighter f1, Fighter f2, int roundsScheduled = 12)
        {
            this.Fighers = new Fighter[] {f1, f2};
            this.RoundsScheduled = roundsScheduled;
            this.Judges = new Judge[] { Judge.RandomJudge(), Judge.RandomJudge(), Judge.RandomJudge() };
        }  

        public Fighter Fighter1()
        {
            return Fighers[0];
        }

        public Fighter Fighter2()
        {
            return Fighers[1];
        }
    }
}
