using System;
using FightSim;
namespace Main
{
    //Holds perm vars about the fight - eventually we should split it out in fight boxscore( total bunches) and fight info (like venue)
    public class Fight
    {
        public readonly int RoundsScheduled;
        public readonly Fighter[] fighters;

        public Judge[] Judges;
        public FightOutcome Outcome;
        public Financials Financials;


        public Fighter FighterRed{ get=> fighters[0];}
        public Fighter FighterBlue { get => fighters[1]; }
        public Fighter Winner { get => Outcome.Winner; }
        public Fighter Loser { get => Outcome.Loser; }

        public Venue Venue { get => Financials.Venue; set => Financials.Venue = value; }

        //Popularity (todo should be moved to a separate class)
        //public double Interested { get; set; }
        //public double Viewership { get; set; }
        //public double Attendance { get; set; }
        //public double TicketPrice { get; set; }



        public Fight()
        {
        }

        public Fight(Fighter f1, Fighter f2, int roundsScheduled = 12)
        {
            this.fighters = new Fighter[] {f1, f2};
            this.RoundsScheduled = roundsScheduled;
            this.Judges = new Judge[] { Judge.RandomJudge(), Judge.RandomJudge(), Judge.RandomJudge() };
        }  
        
        public Fighter Fighter1()
        {
            return fighters[0];
        }

        public Fighter Fighter2()
        {
            return fighters[1];
        }
        
    }
}
