using System;
using FightSim;
using SQLite;

namespace Main
{
    //Holds perm vars about the fight - eventually we should split it out in fight boxscore( total bunches) and fight info (like venue)
    [Table("FightHistory")]
    public class Fight
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int RoundsScheduled { get; }
        public Fighter[] Fighers { get; }

        public Judge[] Judges { get; }
        public FightOutcome Outcome { get; set; }
        public DateTime Date { get; set; }

        public Fight()
        {
        }

        public Fight(Fighter f0, Fighter f1, int roundsScheduled = 12, Judge[] judges = null)
        {
            this.Fighers = new Fighter[] {f0, f1};
            this.RoundsScheduled = roundsScheduled;

            if(judges == null)
                judges = new Judge[] { Judge.RandomJudge(), Judge.RandomJudge(), Judge.RandomJudge() };

            Judges = judges;
        }  

        public Fighter Fighter0()
        {
            return Fighers[0];
        }

        public Fighter Fighter1()
        {
            return Fighers[1];
        }
    }
}
