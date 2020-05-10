using System;
using Fighting;

namespace Main
{
    public class Fighter
    {
        public readonly int id;
        public string name { get; set; } //Uniquely identifies player

        // fighter's elo rating - most recent and before the last fight
        public double elo { get; set; }
        public double previous_elo { get; set; }


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

        public Fighter(string name, double elo = 0): this()
        {
            this.name = name;
            this.elo = this.previous_elo = elo;
        }


        public Fighter()
        {
            JabPercent = 0.000851 * Weight + 0.288; //Regression we did on weight
        }

       
    }
}


