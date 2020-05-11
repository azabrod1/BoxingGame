using System;
using Main;

namespace Fighting
{
    //Represents on minute of time in a fight
    public class Block
    {

        public readonly FighterState f1, f2;
        FightState CurrFight;

        const double PUNCHES_FROM_INTENSITY = 55;   //per fighter!
        readonly double PUNCHES_THROWN_STD = 6.123; //Math.Sqrt(2); //15 * Root(2); as we are adding two normals, we want std for punches thrown to be 15
                                                    // const double PUNCH_DEFICIT_EARLY_FIGHT = 20; //20 less punches in first 3 rounds


        //randoms
        public double BlockIntensity;

        //invisible vars
        public double intensity;

        public Block(FightState currFight)
        {
            this.CurrFight = currFight;
            this.f1 = currFight.f1;
            this.f2 = currFight.f2;
        }

        //Boxers fight!
        public void SimBlock()
        {
            intensity = CalcBlockIntensity();
        }

        /* Intensity of a round is chosen by ring general's aggressiveness
         * Follows normal distribution
         * Average should be about 50, that is average 50 punches out of a
         * round's 105 depend on round intensity
         *
         * Function should return how many punches expected that round given avg fight
         *
         */
        public double CalcBlockIntensity()
        {
            //  double mean = currFight.round < 4 ? PUNCHES_FROM_INTENSITY - PUNCH_DEFICIT_EARLY_FIGHT : PUNCHES_FROM_INTENSITY + PUNCH_DEFICIT_EARLY_FIGHT * 1.3333;


            double mean = PUNCHES_FROM_INTENSITY;

            double aggressiveness = CurrFight.fightControl * f1.AggressionCalc(CurrFight.round) + (1 - CurrFight.fightControl) * f2.AggressionCalc(CurrFight.round);

            /*
            if (b1.RingGen == b2.RingGen)
                aggressiveness = b1.AggressionCalc(CurrFight);
            else
                 aggressiveness = ( b1.RingGen * b1.AggressionCalc(CurrFight) + b2.RingGen * b2.AggressionCalc(CurrFight) ) / (b1.RingGen + b2.RingGen);
                 */

            mean += (aggressiveness - 50) * 0.35;

            BlockIntensity = StatsUtils.Gauss(mean, PUNCHES_THROWN_STD);

            return BlockIntensity;
        }

        public double BoxerPunchesPerRound(FighterState figher)
        {
            //How much they can throw on an avg round?
            double punchMean = figher.PunchCapacity();
            double punchR = StatsUtils.Gauss(punchMean, PUNCHES_THROWN_STD);
            double reachBuff = CurrFight.ReachBuff();
            if (figher == f2)
                reachBuff *= -1;

            return Math.Abs(intensity + punchR) * (1 + reachBuff);
        }

        public double JabPercentages(FighterState fighter, FighterState opponent)
        {
            double prefDistance = fighter.PreferredDistance(opponent);

            double jabMean      = fighter.ExpectedJabRatio();

            //Figher jab percent is affected by preferred distance and what distance the fight is at
            double jabOffset = (prefDistance - 0.5) * 0.70 + (CurrFight.fightDistance-0.5)*0.30;

            double jabMultiplier = StatsUtils.Gauss(1, Constants.JAB_MULTIPLIER_STD);
            jabOffset += jabMultiplier;

            if (jabOffset < 1)
                jabMean /= (2 - jabOffset);
            else
                jabMean *= jabOffset;

            jabMean = Math.Min(1, jabMean);
            jabMean = Math.Max(0, jabMean);

            return jabMean;

        }

        public static double ExpectedAccuracy(FighterState fighter, FighterState opponent)
        {

            return -1;
        }

    }
}
