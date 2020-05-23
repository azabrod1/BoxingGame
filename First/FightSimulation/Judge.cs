using System;
using System.Collections.Generic;
using System.Linq;
using FightSim;
using static FightSim.Block;

namespace Boxing.FightSimulation
{
    public class Judge
    {
        readonly double LandedWeight;
        readonly double CleanessWeight;
        readonly double DamageWeight;

        readonly double IsLandedThreshold;

        public Judge()
        {

        }

        public (int F1Score, int F2Score) ScoreRound(List<PunchResult> punches, FightStats roundStats)
        {
            (int F1Score, int F2Score) roundScore = (10 - roundStats.Knockdowns.Fighter1 , 10 - roundStats.Knockdowns.Fighter2);

            //Auto lose round if knocked down- not strictly the rule but almost always works that way
            if(roundScore.F1Score > roundScore.F2Score)
            {
                roundScore.F2Score--;
                return roundScore;
            }
            else if(roundScore.F1Score < roundScore.F2Score)
            {
                roundScore.F1Score--;
                return roundScore;
            }






            return (2,2);
        }

        private double ScoreFor(List<PunchResult> punches, FightStats roundStats, bool F1)
        {
            double score = DamageWeight * (F1 ? roundStats.Damage.Fighter1 : roundStats.Damage.Fighter2);
            //var f = punches.Where(punch => punch.Accuracy >= IsLandedThreshold )
            //               .Where(punch => punch.ThrownBy == F1? punch.ThrownB)
            
            


            return -1;

        }

    }
}
