using System;
namespace Main
{
    public class FighterRecord
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int KOs { get; set; }
        public int Draws { get; set; }

        public double Rank { get; set; }
        public double PreviousRank { get; set; }

        public FighterRecord()
        {
            Wins = Losses = KOs = Draws = 0;
            Rank = PreviousRank = 0;
        }

        public override string ToString()
        {
            return "rank: " + Rank.ToString("0") + " wins: " + Wins + " with KO: " + KOs + " losses: " + Losses;
        }
    }
}
