using System;
using System.Collections.Generic;
using Main;

namespace FightSim
{
    //Represents on minute of time in a fight
    public class Block
    {
       // private readonly FighterState f1, f2;
        private readonly FighterState[] Boxer;

        readonly FightState Fight;

        const double PUNCHES_FROM_INTENSITY = 55;   //per fighter!
        const double PUNCHES_THROWN_STD = 10;       //Math.Sqrt(2); //15 * Root(2); as we are adding two normals, we want std for punches thrown to be 15

        private double BlockIntensity;
        private double BlockDamage;
        public int[]  Knockdowns = { 0, 0 };


        private (int Jabs, int PowerPunches)[] PunchDistro;

        public Block(FightState fight)
        {
            this.Fight = fight;
            Boxer = new FighterState[] { fight.F1, fight.F2 };
            PunchDistro = new (int Jabs, int PowerPunches)[2];
;
        }

        public class PunchResult
        {
            public readonly int ThrownBy; //Fighter 0 or fighter 1
            public readonly double Damage;
            public readonly double Accuracy;
            public readonly PunchType Punch;

            public PunchResult(int thrownBy, double damage, double accuracy, PunchType punch)
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

            this.PunchDistro[0] = FighterPunchDistribution(Boxer[0], Boxer[1]);
            this.PunchDistro[1] = FighterPunchDistribution(Boxer[1], Boxer[0]);
        }

        /*Throwing more jabs increases your defense
         Need make this functionality cleaner! gross!
        */
        private double DefenseBuffFromJabbing(int defender)
        {
            double jabPercent = 0;

            if(PunchDistro[defender].Jabs != 0 ) //Avoid divide by zero error
                jabPercent = (double)PunchDistro[defender].Jabs / (PunchDistro[defender].PowerPunches + PunchDistro[defender].Jabs);

            double jabBuff = 0.020 * (jabPercent - Constants.JAB_RATIO_AVG);

            //There are some diminishing returns here
            jabBuff = Math.Max(-0.025, Math.Min(0.025, jabBuff));
            return jabBuff;
        }

        /* Boxers fight!
        * Return -1 if no KO
        * Return when in block it happened i.e. punch we at/ total punches this block * 60 (convert to seconds) 
        */
        public (List<PunchResult> Punches, int Stoppage) Play()
        {
            SetRandomVariables(); 

            var punches = CreatePunchSchedule();

            List<PunchResult> punchOutcomes = new List<PunchResult>(punches.Length);

            //Caching for speed

            double[] expectedAcc = new double[]
            {
                ExpectedAccuracy(Boxer[0], Boxer[1]) - DefenseBuffFromJabbing(1),
                ExpectedAccuracy(Boxer[1], Boxer[0]) - DefenseBuffFromJabbing(0)
            };

            double[] expectedPower = new double[]
            {
                Boxer[0].Power() * Boxer[1].DefenseBuff,
                Boxer[1].Power() * Boxer[0].DefenseBuff
            };

            for (int p = 1; p <= punches.Length; ++p)
            {
                (PunchType PunchType, int Attacker) punch = punches[p - 1];

                int attacker = punch.Attacker;
                int defender = 1 - attacker;

                double expectedDamage = expectedPower[attacker] * Math.Max(MathUtils.Gauss(1, 0.75), 1);

                if(punch.PunchType == PunchType.JAB)
                    expectedDamage *= Constants.JAB_POWER;

                //our two big punch stats
                double realizedAcc = MathUtils.Gauss(expectedAcc[attacker], 0.5); //Higher acc means more clean, < 0 shows how bad it missed
                double realizedDamage = realizedAcc == 0? 0 : Math.Max(expectedDamage * realizedAcc, 0) * BlockDamage;

                punchOutcomes.Add(new PunchResult(punch.Attacker, realizedDamage, realizedAcc, punch.PunchType));

                /***    DOWN HE GOES!
                 * A knockdown happens if fighter is 0 health or they eat a shot their chin cannot handle.
                 * They get back up if they have enough time to get positive health 
                 *
                 * ***/
                if (Boxer[defender].IncrementHealth(-realizedDamage) <= 0 || realizedDamage > Boxer[defender].Chin()) 
                {
                    Boxer[defender].RecoverFor(15);               //Knockdown gives you about 15 seconds to recover

                    if (Boxer[defender].Health <= 0)              //THE REF IS GONNA WAVE IT OFF!
                        return (Punches: punchOutcomes, Stoppage: (int)60d * p / punches.Length);  //Second of the block fight stopped
                    else
                        ++Knockdowns[attacker]; //The fighter recovers and its regestered as knockdown
                }
            }

            return (Punches: punchOutcomes, Stoppage: -1);
        }

        /* Given how many power punches and jabs each figher throws,
         * determine the actual sequence the punches will be thrown
         */
        public (PunchType PunchType, int Attacker)[] CreatePunchSchedule()
        {

            (PunchType, int)[] punches = new (PunchType, int)[PunchDistro[0].Jabs
                                                            + PunchDistro[0].PowerPunches
                                                            + PunchDistro[1].Jabs
                                                            + PunchDistro[1].PowerPunches];

            int p = 0;

            for(int fighter = 0; fighter < 2; ++fighter)
            {
                for (int x = 0; x < PunchDistro[fighter].Jabs; ++x)
                    punches[p++] = (PunchType.JAB, fighter);
                for (int x = 0; x < PunchDistro[fighter].PowerPunches; ++x)
                    punches[p++] = (PunchType.POWER_PUNCH, fighter);
            }

            MathUtils.Shuffle(punches);

            return punches;

        }

        private (int, int) FighterPunchDistribution(FighterState fighter, FighterState opponent)
        {
            double totalPunches = BoxerPunchesPerRound(fighter) * 0.333333333; //Since block is 1/3 a round
            double jabPercent = JabPercentages(fighter, opponent);
            
            int jabs = MathUtils.NearestInt(jabPercent * totalPunches);
            int powerPunches = MathUtils.NearestInt((1 - jabPercent) * totalPunches);

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

            double aggressiveness = Fight.FightControl * Boxer[0].AggressionCalc(Fight.Round) + (1 - Fight.FightControl) * Boxer[1].AggressionCalc(Fight.Round);

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
            if (figher == Boxer[1])
                reachBuff *= -1;

            return Math.Abs(BlockIntensity + randomComponent) * (1 + reachBuff);
        }

        public double JabPercentages(FighterState fighter, FighterState opponent)
        {
            double prefDistance = fighter.PreferredDistance(opponent);
            double jabMean      = fighter.ExpectedJabRatio();

            //Figher jab percent is affected by preferred distance and what distance the fight is at
            double jabOffset = MathUtils.WeightedAverage(prefDistance - 0.5,
                                                         Constants.PREF_DISTANCE_WEIGHT_ON_JAB,
                                                         Fight.FightDistance - 0.5,
                                                         1 - Constants.PREF_DISTANCE_WEIGHT_ON_JAB);


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
            double expectedAcc = -0.28 + (attacker.ExpectedAccuracy() - defender.ExpectedDefense()) * 0.0013;
            return expectedAcc;
        }

    }
}
