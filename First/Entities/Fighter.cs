
using System.Collections.Generic;

using System;
using FightSim;

namespace Main
{
    [Serializable]
    public class Fighter
    {
        public string Name { get; internal set; } //Uniquely identifies player #todo lets have a cache of names to ensure no DUPS

        public FighterRecord Record { get; set; }

        //Combat Attributes
        public double Accuracy   { get; set; } = 50;
        public double Aggression { get; set; } = 50;
        public double Defense    { get; set; } = 50;
        public double Durability { get; set; } = 50;
        public double FootWork   { get; set; } = 50;
        public double HandSpeed  { get; set; } = 50;
        public double Power      { get; set; } = 50;
        public double RingGen    { get; set; } = 50;
        public double Stamina    { get; set; } = 50;

        //Physical/Personal Attributes
        public int Reach { get; set; } = 72;
        public int Age { get; set; }   = 25;
        public int AgeCurve { get; internal set; } = FighterAging.ALWAYS_BAD_CURVE;

        public double Weight { get { return _Weight; } set { _Weight = value; JabPercent = ExpectedJabPercent();  } }
        public int Height { get; set; }

        public Country Nationality { get { return _Nationality; } set { _Nationality = value.Name; } }
        public string _Nationality = "United States";

        public Dictionary<string, double> Performance { get; set; } = new Dictionary<string, double>();

        public int Belts; //TODO: This should be stored elsewhere

        //Strategy variables - different fighters will have different stats
        public double DistancePreference = 0.5;
        public double JabPercent { get; internal set; }

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

        public double OverallSkill() //Not exact
        {
            double skill = 0;
            skill += MathUtils.WeightedAverage(Accuracy, 11,
                                               Defense, 11,
                                               Durability, 7,
                                               FootWork, 7,
                                               RingGen, 7,
                                               HandSpeed, 6,
                                               Stamina, 7,
                                               Power, 8
                                              );

            return skill;
        }

        public double OverallSkillP4P() //Not exact
        {
            double skill = 0;
            skill += MathUtils.WeightedAverage(Accuracy, 11,
                                               Defense, 11,
                                               Durability, 7,
                                               FootWork, 7,
                                               RingGen, 7,
                                               HandSpeed, 6,
                                               Stamina, 7,
                                               Power, 8
                                              );

            if (skill > Constants.P4P_BUFF_THRESHOLD)
                skill += 1.3 * (skill - Constants.P4P_BUFF_THRESHOLD);

            return skill;
        }

        public double AverageSkill() 
        {
            double skill = 0;
            skill += MathUtils.WeightedAverage(Accuracy, 10,
                                               Defense, 10,
                                               Durability, 10,
                                               FootWork, 10,
                                               RingGen, 10,
                                               HandSpeed, 10,
                                               Stamina, 10,
                                               Power, 10
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