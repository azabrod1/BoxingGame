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

            //this.DefenseBuff = 0.054*1.75; //Lower is better

            //this.DefenseBuff = 1.546 + Self.Defense * (-0.01211 + 0.0000562 * Self.Defense)) * 0.054;

            this.DefenseBuff = (1.7465 + Self.Defense * (-0.01555 + 0.0000599 * Self.Defense)) * 0.054;
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
            power *= PowerDurabilityWeightBuff(true);
            return power;
        }

        public double ExpectedAccuracy()
        {
            double accuracy = MathUtils.WeightedAverage(Self.Accuracy, 1, Self.HandSpeed,
                                        Constants.HAND_SPEED_ACC_BUFF, Self.FootWork, Constants.FOOT_WORK_ACC_BUFF);


            //After certain point your skills really scale! 
            if (accuracy > Constants.P4P_BUFF_THRESHOLD)
                accuracy += 1 * (accuracy - Constants.P4P_BUFF_THRESHOLD);

            return accuracy;
        }

        public double ExpectedDefense()
        {
            double defense = MathUtils.WeightedAverage(Self.Defense, 1, Self.FootWork, Constants.FOOT_WORK_ACC_BUFF);

            //P4P fighters are really hard to challenge 
            if (defense > Constants.P4P_BUFF_THRESHOLD)
                defense = 1 * (defense - Constants.P4P_BUFF_THRESHOLD);

            return defense;
        }

        //If you get hit with a shot > Chin(), you are knocked down
        public double Chin()
        {
            return RecoveryRate * 150; 
        }

        public double PowerDurabilityFormula(double skill)
        {
            return 3.340 * (skill) + 250; //250 low, 417 avg, 584 hi for avg weight
        }

        //Power and Durability increase with weight, We model this exponential though
        //in real life it might be closer to linear, lets work on that.
        //Bring weight/avg weight to a certain power.
        //Power scales a bit more than durablility, hence HW knockouts are more common
        private double PowerDurabilityWeightBuff(bool PowerBuff)
        {
            double effectiveWeight = Math.Min(Self.Weight, 225); //Lets say it doesnt matter much after that...for now at least
            if (PowerBuff)
                return Math.Pow(effectiveWeight * Constants.AVG_WEIGHT_INV, Constants.WEIGHT_POWER_BUFF);

            return Math.Pow(effectiveWeight * Constants.AVG_WEIGHT_INV, Constants.WEIGHT_DURABILITY_BUFF);

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
            //Power should increase with weight at a slightly higher rate than durability w weight  i.e. HW KOs are more powerful 
            double weightBuff = PowerDurabilityWeightBuff(false);
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
            preferredDistance = MathUtils.WeightedAverage(preferredDistance, Self.RingGen, 0.5, 100 - Self.RingGen);

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

        public static double WinOddsSkill(Fighter f0, Fighter f1)
        {
            double f0Ovrl = f0.OverallSkillP4P();
            double f1Ovrl = f1.OverallSkillP4P();

            double skillDif = f0Ovrl - f1Ovrl;
            if (skillDif > 25)
                return 1;

            if (skillDif < -25)
                return 0;

            double offset = Math.Abs(skillDif) * 0.039 + skillDif * skillDif * -0.0006665;
            if (skillDif < 0)
                offset *= -1;

            return Math.Min(1, Math.Max(0, 0.5 + offset));
        }

    }


}
