using System;
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
        public readonly int TimeOfStoppage; //Minute fight ended... should do seconds eventually, -1 for no KO
        public readonly MethodOfResult Method;
        public readonly Main.Fighter Winner;

        public readonly int[,] Scorecards = new int[3, 2];

        public double Viewership { get; set; } //todo this should prob be on Fight object eventually

        public FightOutcome(int timeOfStoppage, MethodOfResult method, Main.Fighter winner, int[,] scorecards)
        {
            this.TimeOfStoppage = timeOfStoppage; //use double.PositiveInfinity for no KO
            this.Method = method;
            this.Winner = winner;
            this.Scorecards = scorecards;
        }

        public override string ToString()
        {
            string ret = String.Format($"Winner: {Winner}, Method: {Method}," +
                $" Scorecards: {Scorecards[0,0]}-{Scorecards[0,1]} {Scorecards[1,0]}-{Scorecards[1,1]} {Scorecards[2,0]}-{Scorecards[2,1]}");

            if (this.IsKO())
                ret += String.Format(", Time of stoppage: Round {0}, Time: {1}", RoundOfStoppage(), TimeOfStoppage - RoundOfStoppage() * 180);

            return ret; 

        }

        public int RoundOfStoppage()
        {
            return TimeOfStoppage / 180;
        }

    }
}
