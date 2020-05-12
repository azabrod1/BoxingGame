using System;
using FightSim;
namespace Main
{
    //Holds perm vars about the fight - eventually we should split it out in fight boxscore( total bunches) and fight info (like venue) 
    public class Fight
    {
        public readonly int roundsScheduled;
        public readonly Fighter b1;
        public readonly Fighter b2;

        public string[] judges;
        public FightOutcome outcome; 
        
        public Fight(Fighter b1, Fighter b2, int roundsScheduled = 12)
        {
            this.b1 = b1;
            this.b2 = b2;
            this.roundsScheduled = roundsScheduled;
        }  

    }
}
