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
        public readonly double TimeOfStoppage; //Minute fight ended... should do seconds eventually, -1 for no KO
        public readonly MethodOfResult Method;
        public readonly Main.Fighter Winner;

        public readonly int[,] Scorecards = new int[3,2]; 

        public FightOutcome(double timeOfStoppage, MethodOfResult method, Main.Fighter winner)
        {
            this.TimeOfStoppage = timeOfStoppage; //use double.PositiveInfinity for no KO
            this.Method = method;
            this.Winner = winner;
        }
     
    }
}
