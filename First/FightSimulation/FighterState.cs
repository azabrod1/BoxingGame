using System;
using Main;

namespace FightSim
{
    public class FighterState
    {
        Fighter Self;
        public double Health { get; set; }
        public double RecoveryRate;
        public double DefenseBuff { get; set; } //The better your defensive, the less cleanly opponent punches land - the less damage you take

        public FighterState(Fighter fighter)
        {
            this.Self = fighter;
            this.Health = GetDurability();
            this.RecoveryRate = CalcRecoveryRate();

            //As fighters get better, they know how to avoid taking clean damaging shots
            //I put it in because we need something to counter the fact that as ppl get better they throw more otherwise
            //high skilled fighters would always get KO, while its usually other way around, BUMs ko eachother

            this.DefenseBuff = (1.546 + Self.Defense * (-0.01211 + 0.0000562 * Self.Defense)) * 0.054;
        }

        public double IncrementHealth(double change)
        {
            Health = Math.Min(change + Health, GetDurability());
            return Health;
        }

        public double RecoverFor(double seconds)
        {
            IncrementHealth(RecoveryRate * seconds);
            return Health;
        }

        //How much damage will I do in a round? 
        public double Power()
        {
            double power = PowerSkillFormula(Self.Power);
            power *= Self.Weight * Constants.AVG_WEIGHT_INV;
            return power;
        }

        public double ExpectedAccuracy()
        {
            double accuracy = MathUtils.WeightedAverage(Self.Accuracy, 1, Self.HandSpeed, Constants.HAND_SPEED_ACC_BUFF, Self.FootWork, Constants.FOOT_WORK_ACC_BUFF);
            return accuracy;
        }

        public double ExpectedDefense()
        {
            double defense = MathUtils.WeightedAverage(Self.Defense, 1, Self.FootWork, Constants.FOOT_WORK_ACC_BUFF);
            return defense;
        }

        //If you get hit with a shot > Chin(), you are knocked down
        public double Chin()
        {
            return RecoveryRate * 180; 
        }

        public double PowerDurabilityFormula(double skill)
        {
            return 3.340 * (skill) + 250; //250 low, 417 avg, 584 hi for avg weight
        }

        //The below two formulas are similar
        //but durability increases a bit more as your quality goes up to make up for stamina and speed increasing at high level
        public double PowerSkillFormula(double skill)
        {
            return 3.340 * (skill) + 250; //250 low, 417 avg, 584 hi for avg weight
        }

        public double DurabilitySkillFormula(double skill)
        {
           // return 4.1 * (skill) + 200; //250 low, 417 avg, 584 hi for avg weight
            return 3.340 * (skill) + 250; //250 low, 417 avg, 584 hi for avg weight
        }

        public double GetDurability()
        {
            const double WEIGHT_DURABILITY_BUFF = 0.8;
            //Power should increase with weight at a slightly higher rate than durability w weight  i.e. HW KOs are more powerful 
            double weightBuff = MathUtils.WeightedAverage(Self.Weight * Constants.AVG_WEIGHT_INV, WEIGHT_DURABILITY_BUFF, 1, 1 - WEIGHT_DURABILITY_BUFF);
            double durability = DurabilitySkillFormula(Self.Durability) * weightBuff;
            return durability;
        }

        //How much fighters are expected to recover per second
        private double CalcRecoveryRate()
        {
            return GetDurability() * 0.00104166666;   //Divide by 16 * 60 (4 Mins) - in other words you recover fully in 4 rounds if you took no damage in them
        }

        public double AggressionCalc(int round)
        {
            double agg = round <= 1 ? Self.Aggression * .5 : Self.Aggression * 1.1;
            return agg;
        }

        public double PreferredDistance(FighterState opponent)
        {
            double reachDifference = Self.Reach - opponent.Self.Reach;
            double absReachDifference = Math.Abs(reachDifference) - 0.7; //small reach adv does not rly matter
            if (absReachDifference < 0)
                absReachDifference = 0;

            double preferredDistance = 1.0 / (1.0 + Math.Pow(Constants.CUBE_ROOT_TWO, absReachDifference));

            preferredDistance = reachDifference >= 0 ? 1 - preferredDistance : preferredDistance;

            //You don't know how to set up distance if your ring IQ is low
            preferredDistance = preferredDistance * (Self.RingGen / 100.0) + 0.5 * ((100 - Self.RingGen) / 100.0);

            return preferredDistance;
        }

        //How many punches/ feints etc can you do?
        //We will say that a 150 fighter should land about 55 per round
        //5 points of stamina get you an extra punch
        //Every 10 Pounds, you lose a punch
        //Higher weight requires more stamina to throw punches

        public const double PUNCH_FUDGE = 2.5;
        public double PunchCapacity()
        {
            double realStamina = PUNCH_FUDGE + 0.2 * (Self.Stamina + Self.HandSpeed) - Self.Weight * 0.15; //we should consider doing .15 for all these
            if (Self.Weight > 190)
                realStamina += (Self.Weight - 190) * .05;
            return realStamina;
        }

        public double ExpectedJabRatio()
        {
            return Self.JabPercent;
        }

        public string Name()
        {
            return Self.Name;
        }

    }


}
