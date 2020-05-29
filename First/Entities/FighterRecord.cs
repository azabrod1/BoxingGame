using System;
namespace Main
{
    public class FighterRecord
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int KOs { get; set; }
        public int Draws { get; set; }

        private double _rank;

        public double Rank {
            get { return _rank; }
            set { PreviousRank = _rank; _rank = value; }
            }

        public double PreviousRank { get; private set; } // automatically preserved


        public double Elo // synonym to Rank... LOL
        {
            get { return Rank; }
            set { Rank = value; }
        }

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
