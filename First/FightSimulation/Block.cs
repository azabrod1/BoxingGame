using System;
using Main;

namespace FightSim
{
    //Represents on minute of time in a fight
    public class Block
    {
        private readonly FighterState f1, f2;
        FightState Fight;

        const double PUNCHES_FROM_INTENSITY = 55;   //per fighter!
        const double PUNCHES_THROWN_STD = 10;  //Math.Sqrt(2); //15 * Root(2); as we are adding two normals, we want std for punches thrown to be 15

        private double BlockIntensity;

        public Block(FightState fight)
        {
            this.Fight = fight;
            this.f1 = fight.f1;
            this.f2 = fight.f2;
        }

        enum PunchThrown
        {
            F1_JAB,
            F1_PP,
            F2_JAB,
            F2_PP,
         };

        //Boxers fight!
        public double Play()
        {
            //How intense will this minute of the fight be?
            BlockIntensity = CalcBlockIntensity();
            PunchThrown[] punches = GeneratePunchDistribution();



            return -1;
        }

        private PunchThrown[] GeneratePunchDistribution()
        {
            (int, int) f1Punches = FighterPunchDistribution(f1, f2);
            (int, int) f2Punches = FighterPunchDistribution(f2, f1);

            PunchThrown[] punches = new PunchThrown[f1Punches.Item1 + f1Punches.Item2 + f2Punches.Item1 + f2Punches.Item2];
            int idx = 0;
            for (int x = 0; x < f1Punches.Item1; ++x)
                punches[idx++] = PunchThrown.F1_JAB;
            for (int x = 0; x < f1Punches.Item2; ++x)
                punches[idx++] = PunchThrown.F1_PP;
            for (int x = 0; x < f2Punches.Item1; ++x)
                punches[idx++] = PunchThrown.F2_JAB;
            for (int x = 0; x < f2Punches.Item2; ++x)
                punches[idx++] = PunchThrown.F2_PP;

            MathUtils.Shuffle(punches);

            return punches;

        }

        private (int, int) FighterPunchDistribution(FighterState fighter, FighterState opponent)
        {
            double totalPunches = BoxerPunchesPerRound(fighter) * 0.333333333; //Since block is 1/3 a round
            double jabPercent = JabPercentages(fighter, opponent);
            int jabs = (int)(jabPercent * totalPunches + 0.5);
            int powerPunches = (int)((1 - jabPercent) * totalPunches * totalPunches + 0.5);

            return (jabs, powerPunches);
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
            double mean = PUNCHES_FROM_INTENSITY;

            double aggressiveness = Fight.fightControl * f1.AggressionCalc(Fight.Round) + (1 - Fight.fightControl) * f2.AggressionCalc(Fight.Round);

            mean += (aggressiveness - 50) * 0.35;

            return MathUtils.Gauss(mean, PUNCHES_THROWN_STD);
        }

        //How much would they throw this round if the entire round was like this block
        //Should be divided by how many blocks in a round
        public double BoxerPunchesPerRound(FighterState figher)
        {
            double figherMean      = figher.PunchCapacity();
            double randomComponent = MathUtils.Gauss(figherMean, PUNCHES_THROWN_STD);
            double reachBuff       = Fight.ReachBuff();
            if (figher == f2)
                reachBuff *= -1;

            return Math.Abs(BlockIntensity + randomComponent) * (1 + reachBuff);
        }

        public double JabPercentages(FighterState fighter, FighterState opponent)
        {
            double prefDistance = fighter.PreferredDistance(opponent);

            double jabMean      = fighter.ExpectedJabRatio();

            //Figher jab percent is affected by preferred distance and what distance the fight is at
            double jabOffset = (prefDistance - 0.5) * 0.70 + (Fight.FightDistance-0.5)*0.30;

            double jabMultiplier = MathUtils.Gauss(1, Constants.JAB_MULTIPLIER_STD);
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
