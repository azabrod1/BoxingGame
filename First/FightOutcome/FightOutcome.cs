using System;
using Main;

namespace FightSim
{
    /*
     * IDK if needed
    public enum Winner{
        None = 0,
        Player1 = 1,
        Player2 = 2,
    }*/

    /* This object will be stamped on the Fight object after a fight

    */

    public class FightOutcome
    {
        public double Interested { get; set; }
        public readonly int TimeOfStoppage; //Minute fight ended... should do seconds eventually, -1 for no KO
        public readonly MethodOfResult Method;
        public readonly Fighter[] Fighters;

        private int _winner = -1;

        public readonly int[,] Scorecards = new int[3, 2];

        public double Viewership { get; set; } //todo this should prob be on Fight object eventually

        public FightOutcome(int timeOfStoppage, MethodOfResult method, Main.Fighter winner, int[,] scorecards, Fighter[] fighters)
        {
            this.TimeOfStoppage = timeOfStoppage; //use double.PositiveInfinity for no KO
            this.Method = method;
            this.Scorecards = scorecards;
            this.Fighters = fighters;
            this.Winner = winner;

        }

        public FightOutcome(int timeOfStoppage, MethodOfResult method, Main.Fighter winner, int[,] scorecards, Fight fight)
        :
        this(timeOfStoppage, method, winner, scorecards, fight.Fighers)
        {
        
        }

        public override string ToString()
        {
            string ret = String.Format($"Winner: {Winner}, Method: {Method}," +
                $" Scorecards: {Scorecards[0,0]}-{Scorecards[0,1]} {Scorecards[1,0]}-{Scorecards[1,1]} {Scorecards[2,0]}-{Scorecards[2,1]}");

            if (this.IsKO())
                ret += String.Format(", Time of stoppage: Round {0}, Time: {1}", RoundOfStoppage(), TimeOfStoppage - RoundOfStoppage() * 180);

            return ret; 

        }

        public bool IsDraw()
        {
            return Winner == null;
        }

        public int WinnerNum()
        {
            return _winner;
        }

        public Fighter Winner
        {
            get
            {
                if (_winner == -1)
                    return null;
                return Fighters[_winner];
            }

            set
            {
                if (value == null)
                    _winner = -1;
                else
                    _winner = value == Fighters[0]? 0 : 1;
            }
        }

        public bool IsKO()
        {
            return Method == MethodOfResult.KO || Method == MethodOfResult.TKO;
        }

        public int RoundOfStoppage()
        {
            return TimeOfStoppage / 180;
        }

        public Fighter Fighter1()
        {
            return Fighters[0];
        }

        public Fighter Fighter2()
        {
            return Fighters[1];
        }

    }
}
