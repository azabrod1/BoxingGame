using System;
namespace Main
{
    public class FighterRecord
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int KOs { get; set; }
        public int Draws { get; set; }

        public FighterRecord()
        {
            Wins = Losses = KOs = Draws = 0;
        }

        public override string ToString()
        {
            return "wins: " + Wins + " with KO: " + KOs + " losses: " + Losses;
        }
    }
}
