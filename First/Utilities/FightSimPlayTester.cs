using System;
using System.Collections.Generic;
using System.Text;
using Boxing.FighterRating;
using FightSim;
using Main;

namespace Utilities
{
    public class FightSimPlayTester
    {
        public FighterCache Fighters;
        private readonly FightSimulator FightSim = new FightSimulatorGauss();
        double MatchCoef { get; set; } = 0.1; //Higher values mean more likely fighters someone as good as them
        IPlayerRating Elo;

        public FightSimPlayTester()
        {
            this.Fighters = new FighterCache();
            this.Elo = new EloFighterRating();
        }

        public void SimFights(int numFights)
        {

            for (int fightNight = 0; fightNight < numFights; ++fightNight)
            {
                List<Fight> fights = ScheduleFights();
                var outcomes = FightSim.SimulateManyFights(fights);
                foreach(var outcome in outcomes)
                {
                    outcome.Fighters[0].UpdateRecord(outcome);
                    outcome.Fighters[1].UpdateRecord(outcome);
                    Elo.CalculateRatingChange(outcome);
                }
            }
        }

        //Top: how many of the top guys to display
        public string Status(int top = -1)
        {
            StringBuilder sb = new StringBuilder();
            if (top == -1)
                top = Fighters.Count();

            List<Fighter> fighters = Fighters.AllFighters();

            //Sort in reverse order
            fighters.Sort((f1, f2) => Elo.Rating(f2).CompareTo(Elo.Rating(f1)));

            for(int f = 0; f < top; ++f)
            {
                Fighter curr = fighters[f];
                sb.AppendFormat($"{f+1}. {curr.Name}\n\t{curr.Record} Rating: {Elo.Rating(curr)}\n");
                sb.AppendFormat($"\tSkill Level {curr.OverallSkill()}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private List<Fight> ScheduleFights()
        {
            List<Fighter> fighters = Fighters.AllFighters();
            List<Fight> schedule = new List<Fight>();

            //Sort in reverse order
            fighters.Sort((f1, f2) => Elo.Rating(f2).CompareTo(Elo.Rating(f1)));

            while (fighters.Count != 0)
            {
                int f1 = 0, f2 = 1;
                for (; f2 < fighters.Count - 1; ++f2)
                    if (MathUtils.RangeUniform(0, 1) < MatchCoef)
                        break;

                schedule.Add(new Fight(fighters[f1], fighters[f2]));
                fighters.RemoveAt(f1); fighters.RemoveAt(f2-1);
            }

            return schedule;
        }

        public List<Fighter> AddFighters(int numFighters, int weight)
        {
            List<Fighter> added = new List<Fighter>();
            for (int f = 0; f < numFighters; ++f)
                added.Add(Fighters.CreateRandomFighter(weight));

            Elo.AddFighters(added);


            return added;
        }



    }
}
