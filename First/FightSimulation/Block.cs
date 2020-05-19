using System;
using System.Collections.Generic;
using Main;

namespace FightSim
{
    //Represents on minute of time in a fight
    public class Block
    {
        private readonly FighterState f1, f2;
        FightState Fight;

        const double PUNCHES_FROM_INTENSITY = 55;   //per fighter!
        const double PUNCHES_THROWN_STD = 10;       //Math.Sqrt(2); //15 * Root(2); as we are adding two normals, we want std for punches thrown to be 15

        private double BlockIntensity;
        private double BlockDamage;

        public Block(FightState fight)
        {
            this.Fight = fight;
            this.f1 = fight.F1;
            this.f2 = fight.F2;
        }

        public class PunchResult
        {
            public readonly FighterState ThrownBy;
            public readonly double Damage;
            public readonly double Accuracy;
            public readonly PunchType Punch;

            public PunchResult(FighterState thrownBy, double damage, double accuracy, PunchType punch)
            {
                this.ThrownBy = thrownBy;
                this.Damage   = damage;
                this.Accuracy = accuracy;
                this.Punch    = punch;
            }

        }

        public void SetRandomVariables()
        {
            //How intense will this minute of the fight be?
            this.BlockIntensity = CalcBlockIntensity();

            //How damaging will the exchanges be? todo Similar to the above; consider combining
            this.BlockDamage = Math.Max(0.5, MathUtils.Gauss(1, 0.75));
        }

        /* Boxers fight!
        * Return -1 if no KO
        * Return when in block it happened i.e. punch we at/ total punches this block * 60 (convert to seconds) 
        */
        
        public (List<PunchResult> Punches, int Stoppage) Play()
        {
            SetRandomVariables(); //Seperated into a dif function mainly for testing

            (PunchType, FighterState)[] punches = GeneratePunchDistribution();

            List<PunchResult> punchOutcomes = new List<PunchResult>(punches.Length);

            //Caching for speed
            double expectedAccF1 = ExpectedAccuracy(f1, f2);
            double expectedAccF2 = ExpectedAccuracy(f2, f1);

            for (int p = 1; p <= punches.Length; ++p)
            {
                (PunchType, FighterState) punch = punches[p - 1];
                FighterState attacker = punch.Item2;
                FighterState defender = attacker == f1 ? f2 : f1;

                double expectedDamage = Math.Max(MathUtils.Gauss(1, 0.75), 1) * attacker.Power();

                if (punch.Item1 == PunchType.JAB)
                    expectedDamage *= Constants.JAB_POWER;

                //our two big punch stats
                double expectedAcc = attacker == f1 ? expectedAccF1 : expectedAccF2;
                double realizedAcc    = Math.Max(0, MathUtils.Gauss(expectedAcc, 0.5));
                double realizedDamage = Math.Max(expectedDamage * realizedAcc, 0) * BlockDamage;

                punchOutcomes.Add(new PunchResult(attacker, realizedDamage, realizedAcc, punch.Item1));

                /***    DOWN HE GOES!
                 * A knockdown happens if fighter is 0 health or they eat a shot their chin cant handle.
                 * They get back up if they have enough time to get positive health 
                 *
                 * ***/
                if (defender.IncrementHealth(-realizedDamage) <= 0 || realizedDamage > defender.Chin()) 
                {          
                    defender.RecoverFor(15); //Knockdown gives you about 15 seconds to recover

                    if (defender.Health <= 0) //THE REF IS GONNA WAVE IT OFF!
                        return (Punches: punchOutcomes, Stoppage: (int) 60d * p / punches.Length);  //Basically which second of the block was the stopage
                    else
                                             //The fighter recovers and its regestered as knockdown
                        Fight.RegisterKnockdown(attacker);
                }
            }

            return (Punches: punchOutcomes, Stoppage: -1);
        }

        public (PunchType, FighterState)[] GeneratePunchDistribution()
        {
            (int, int) f1Punches = FighterPunchDistribution(f1, f2);
            (int, int) f2Punches = FighterPunchDistribution(f2, f1);

            (PunchType, FighterState)[] punches = new (PunchType, FighterState)[f1Punches.Item1 + f1Punches.Item2 + f2Punches.Item1 + f2Punches.Item2];

            (PunchType, FighterState) F1_Jab = (PunchType.JAB, f1);
            (PunchType, FighterState) F2_Jab = (PunchType.JAB, f2);
            (PunchType, FighterState) F1_PP = (PunchType.POWER_PUNCH, f1);
            (PunchType, FighterState) F2_PP = (PunchType.POWER_PUNCH, f2);

            int p = 0;
            for (int x = 0; x < f1Punches.Item1; ++x)
                punches[p++] = F1_Jab;
            for (int x = 0; x < f1Punches.Item2; ++x)
                punches[p++] = F1_PP;
            for (int x = 0; x < f2Punches.Item1; ++x)
                punches[p++] = F2_Jab;
            for (int x = 0; x < f2Punches.Item2; ++x)
                punches[p++] = F2_PP;

            MathUtils.Shuffle(punches);

            return punches;

        }

        private (int, int) FighterPunchDistribution(FighterState fighter, FighterState opponent)
        {
            double totalPunches = BoxerPunchesPerRound(fighter) * 0.333333333; //Since block is 1/3 a round
            double jabPercent = JabPercentages(fighter, opponent);
            int jabs = (int)(jabPercent * totalPunches + 0.5);
            int powerPunches = (int)((1 - jabPercent) * totalPunches + 0.5);

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

            double aggressiveness = Fight.FightControl * f1.AggressionCalc(Fight.Round) + (1 - Fight.FightControl) * f2.AggressionCalc(Fight.Round);

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

        public static double ExpectedAccuracy(FighterState attacker, FighterState defender)
        {
            double expectedAcc = -0.28 + (attacker.ExpectedAccuracy() - defender.ExpectedDefense()) * 0.0012;
            return expectedAcc;
        }

    }
}
