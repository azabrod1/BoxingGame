using System;
using Main;

namespace Fighting
{
    public class FighterState
    {
        Fighter Self;
        public double Health { get; set; }

        public FighterState(Fighter fighter)
        {
            this.Self = fighter;
            this.Health = GetDurability();
        }

        public double IncrementHealth(double change)
        {
            Health = Math.Min(change + Health, GetDurability() );
            return Health;
        }

        //How much damage will I do in a round? 
        public double Power()
        {
            double power = PowerDurabilityFormula(Self.Power) * 0.045;
            power *= Self.Weight * Constants.AVG_WEIGHT_INV;
            return power;
        }

        public double ExpectedAccuracy()
        {
            return Self.Accuracy + Constants.HAND_SPEED_ACC_BUFF* Self.HandSpeed;
        }

        public double ExpectedDefense()
        {
            return (1+Constants.HAND_SPEED_ACC_BUFF)*Self.Defense;
        }

        public double PowerDurabilityFormula(double skill)
        {
            return 3.340 * (skill) + 250; //250 low, 417 avg, 584 hi for avg weight
        }

        public double GetDurability()
        {
            const double WEIGHT_DURABILITY_BUFF = 0.8;
            //Power should increase with weight at a slightly higher rate than durability w weight  i.e. HW KOs are more powerful 
            double weightBuff = Utility.WeightedAverage(Self.Weight * Constants.AVG_WEIGHT_INV, WEIGHT_DURABILITY_BUFF, 1, 1-WEIGHT_DURABILITY_BUFF);
           // Console.WriteLine(weightBuff);
            return PowerDurabilityFormula(Self.Durability)*weightBuff;
        }

        public double RecoveryRate()
        {
              return GetDurability() * 0.0625;   //Divide by 16
          //  return getDurability() * 0.083333; //divide by 12
        }

        public double AggressionCalc(int round)
        {
            double agg = round <= 2 ? Self.Aggression * .5 : Self.Aggression * 1.1;
            return agg;
        }

        public double PreferredDistance(FighterState opponent)
        {
            double reachDifference    = Self.Reach - opponent.Self.Reach;
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
            double jabPercentNormal = Self.JabPercent;
            return Self.JabPercent;
        }

    }


}
