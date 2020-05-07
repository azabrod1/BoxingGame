using System;
using Main;

namespace Fighting
{
    public class FighterState
    {
        Fighter self;
        public double health { get; set; }

        public FighterState(Fighter fighter)
        {
            this.self = fighter;
            this.health = getDurability();
        }

        public double incrementHealth(double change)
        {
            health = Math.Min(change + health, getDurability() );
            return health;
        }

        //How much damage will I do in a round? 
        public double Power()
        {
            double powerBuff = (self.Power - 50) * 0.004 + 0.8;
            return 0.5*(self.Weight + powerBuff);
        }

        public double WeightPowerBuff()
        {
            double _weight = Math.Min(self.Weight, 245); //For now, weight ain't help you after 245
            double buff = 0.5 * self.Weight;
            return buff;
        }

        public double getDurability()
        {
            return 150 + 2 * self.Durability;
        }

        public double RecoveryRate()
        {
            return getDurability() * 0.083333; //divide by 12
        }

        public double AggressionCalc(int round)
        {
            double agg = round <= 2 ? self.Aggression * .5 : self.Aggression * 1.1;
            return agg;
        }

        public double PreferredDistance(FighterState opponent)
        {
            double reachDifference    = self.Reach - opponent.self.Reach;
            double absReachDifference = Math.Abs(reachDifference) - 0.7; //small reach adv does not rly matter
            if (absReachDifference < 0)
                absReachDifference = 0;

            double preferredDistance = 1.0 / (1.0 + Math.Pow(Constants.CubeRootTwo, absReachDifference));

            preferredDistance = reachDifference >= 0 ? 1 - preferredDistance : preferredDistance;

            //You don't know how to set up distance if your ring IQ is low
            preferredDistance = preferredDistance * (self.RingGen / 100.0) + 0.5 * ((100 - self.RingGen) / 100.0);

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
            double realStamina = PUNCH_FUDGE + 0.2 * (self.Stamina + self.HandSpeed) - self.Weight * 0.15; //we should consider doing .15 for all these
            if (self.Weight > 190)
                realStamina += (self.Weight - 190) * .05;
            return realStamina;
        }

        public double ExpectedJabRatio()
        {
            double jabPercentNormal = self.JabPercent;
            return self.JabPercent;
        }

    }


}
