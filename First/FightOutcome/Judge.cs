using System;
using System.Collections.Generic;
using FightSim;
using Main;
using static FightSim.Block;

namespace FightSim
{
    public class Judge
    {
        readonly double LandedWeight;
        readonly double CleanessWeight;
        readonly double DamageWeight;

        /* Some judges may have negative threshold, counting things that almost land! */
        readonly double IsLandedThreshold;

        public Judge(double landedWeight, double cleanessWeight, double damageWeight, double isLandedThreshold = 0)
        {
            this.LandedWeight = landedWeight;
            this.CleanessWeight = cleanessWeight;
            this.DamageWeight = damageWeight;
            this.IsLandedThreshold = isLandedThreshold;
        }

        public static Judge RandomJudge()
        {
            double isLandedThreshold = Main.MathUtils.Gauss(0, 0.03);

            double damageWeight   = Math.Max( 0, Main.MathUtils.Gauss(2, 1));
            double landedWeight   = 1.25 * Math.Max( 0, Main.MathUtils.Gauss(2, 1));
            double cleanessWeight = Math.Max( 0, Main.MathUtils.Gauss(2, 1));

            return new Judge(landedWeight, cleanessWeight, damageWeight, isLandedThreshold);
        }

        public int[] ScoreRound(List<PunchResult> punches, FightStats roundStats)
        {
            int[] roundScore = { 10 - roundStats.Knockdowns.Fighter1, 10 - roundStats.Knockdowns.Fighter2 };

            //Auto lose round if knocked down- not strictly the rule but almost always works that way
            if(roundScore[0] > roundScore[1])
            {
                roundScore[1]--;
                return roundScore;
            }
            else if (roundScore[0] < roundScore[1])
            {
                roundScore[0]--;
                return roundScore;
            }

            //Ratio of fighter 1s sucessess to fighter 2s
            double combatRatio;

            //I added the plus 20 so that the damage ratio doesnt matter if nobody did any real damage
            double damageRatio = MathUtils.SafeDivide(roundStats.Damage.Fighter1+20,roundStats.Damage.Fighter2+20, 1,10);
            damageRatio = Math.Pow(damageRatio, DamageWeight);

            double[] punchesIThinkLanded = {0,0};
            double[] cleanessSum = {0,0};

            foreach (PunchResult punch in punches)
            {
                int fighter = punch.ThrownBy;

                cleanessSum[fighter] += Math.Max(0, punch.Accuracy);

                if (punch.Accuracy >= IsLandedThreshold)
                    punchesIThinkLanded[fighter]++;
            }

            double landedRatio = MathUtils.SafeDivide(punchesIThinkLanded[0], punchesIThinkLanded[1], 1, 10);
            landedRatio = Math.Pow(landedRatio, LandedWeight);

            double cleannessRatio = MathUtils.SafeDivide(cleanessSum[0], cleanessSum[1], 1, 10);
            cleannessRatio = Math.Pow(cleannessRatio, CleanessWeight);

            combatRatio = damageRatio * landedRatio * cleannessRatio;

            if (combatRatio > 1)
                --roundScore[1];
            else if (combatRatio < 1)
                --roundScore[0];

            return roundScore;
        }

    }
}
