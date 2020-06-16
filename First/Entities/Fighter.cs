using System;
using System.ComponentModel;
using System.Text;
using FightSim;

namespace Main
{
    [ Serializable]
    public class Fighter
    {
        public string Name { get; } //Uniquely identifies player #todo lets have a cache of names to ensure no DUPS

        public FighterRecord Record { get; set; } 

        //Combat Attributes
        public int Accuracy { get; set; } = 50;
        public int Aggression { get; set; } = 50;
        public int Defense { get; set; } = 50;
        public int Durability { get; set; } = 50;
        public int FootWork { get; set; } = 50;
        public int Power { get; set; } = 50;
        public int RingGen { get; set; } = 50;
        public int HandSpeed { get; set; } = 50;
        public int Stamina { get; set; } = 50;

        //Physical/Personal Attributes
        public int Reach { get; set; } = 72;
        public double Weight { get { return _Weight; } set { _Weight = value; JabPercent = ExpectedJabPercent();  } }
        public int Height { get; set; }
        public Country Nationality { get { return _Nationality; } set { _Nationality = value.Name; } }
        public string _Nationality = "United States";



        //Strategy variables - different fighters will have different stats
        public double DistancePreference = 0.5;
        public double JabPercent;

        private double _Weight = 150;

        private Fighter()
        {

        }

        public Fighter(string name = "")
        {
            JabPercent = ExpectedJabPercent();

            Record = new FighterRecord();

            if (string.IsNullOrEmpty(name)) //For testing 
                this.Name = Utility.RandomNameSimple();
            else
                this.Name = name;
        }

        public double OverallSkill() //Not exact
        {
            double skill = 0;
            skill += MathUtils.WeightedAverage(Accuracy, 11,
                                               Defense,  11,
                                               Durability, 7,
                                               FootWork, 7,
                                               RingGen, 7,
                                               HandSpeed, 6,
                                               Stamina, 7,
                                               Power, 8
                 );

            return skill;
        }

        public void UpdateRecord(FightSim.FightOutcome outcome)
        {
            if (outcome.IsDraw())
                ++Record.Draws;
            else if (outcome.Winner == this)
            {
                ++Record.Wins;
                if (outcome.IsKO())
                    ++Record.KOs;
            }
            else
                ++Record.Losses;
        }

        private double ExpectedJabPercent()
        {
            double effectiveWeight = Math.Min(200, Weight);
            effectiveWeight = Math.Max(115, effectiveWeight);

            return 0.000851 * effectiveWeight + 0.288; //Regression we did on weight
        }

        public override string ToString()
        {
            return Utility.UsefulString(this);
        }

    }
}


