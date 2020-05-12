using System;
namespace Main
{
    public class FighterRecord
    {
        public int Wins {get; set;}
        public int Losses {get; set; }
        public int WinsKO { get; set; }
        public int Draws { get; set; }
        


        public FighterRecord()
        {
            Wins = Losses = WinsKO = Draws = 0;
        }

        public string toString()
        {
            return "wins: " + Wins + " with KO: " + WinsKO + " losses: " + Losses;
        }


    }
}
