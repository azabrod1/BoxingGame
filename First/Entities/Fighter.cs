using System;
using FightSim;

namespace Main
{
    public class Fighter
    {
        public readonly string Name; //Uniquely identifies player #todo lets have a cache of names to ensure no DUPS

        public FighterRecord Record { get; set; } 

        //Attributes
        public int Accuracy { get; set; } = 50;
        public int Aggression { get; set; } = 50;
        public int Chin { get; set; } = 50;
        public int Defense { get; set; } = 50;
        public int Durability { get; set; } = 50;
        public int FootWork { get; set; } = 50;
        public int Height { get; set; }
        public int Power { get; set; } = 50;
        public int Reach { get; set; } = 72;
        public int RingGen { get; set; } = 50;
        public int HandSpeed { get; set; } = 50;
        public int Stamina { get; set; } = 50;
        public double Weight { get { return _Weight; } set { _Weight = value; JabPercent = ExpectedJabPercent();  } }

        //Strategy variables - different fighters will have different stats
        public double DistancePreference = 0.5;
        public double JabPercent;

        private double _Weight = 150;

        public Fighter(string name = "")
        {
            JabPercent = ExpectedJabPercent();

            Record = new FighterRecord();

            if (string.IsNullOrEmpty(name)) //For testing 
                this.Name = Utility.RandomNameSimple();
            else
                this.Name = name;
        }

        private double ExpectedJabPercent()
        {
            double effectiveWeight = Math.Min(200, Weight);
            effectiveWeight = Math.Max(115, effectiveWeight);

            return 0.000851 * effectiveWeight + 0.288; //Regression we did on weight
        }

        public override string ToString()
        {
            return this.Name + "(rank = " + this.Record.Rank.ToString("0") + ")";
        }

    }
}


