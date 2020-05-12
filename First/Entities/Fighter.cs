using System;
using FightSim;

namespace Main
{
    public class Fighter
    {
        public readonly string Name; //Uniquely identifies player #todo lets have a cache of names to ensure no DUPS

        public double Rank { get; set; } //  Elo rank
        public FighterRecord record { get; set; } 

        //Attributes
        public int Accuracy { get; set; }
        public int Aggression { get; set; }
        public int Chin { get; set; }
        public int Defense { get; set; }
        public int Durability { get; set; } = 50;
        public int FootWork { get; set; }
        public int Height { get; set; }
        public int Power { get; set; } = 50;
        public int Reach { get; set; }
        public int RingGen { get; set; }
        public int HandSpeed { get; set; }
        public int Stamina { get; set; }
        public double Weight { get; set; } = 150;

        //Strategy variables - different fighters will have different stats
        public double DistancePreference = 0.5;
        public double JabPercent;

        public Fighter(string name = "")
        {
            JabPercent = 0.000851 * Weight + 0.288; //Regression we did on weight

            record = new FighterRecord();

            if (string.IsNullOrEmpty(name)) //For testing 
                this.Name = Utility.RandomNameSimple();
            else
                this.Name = name;
        }

        public override string ToString()
        {
            return this.Name + "(rank = " + this.Rank.ToString("0") + ")";
        }

    }
}


