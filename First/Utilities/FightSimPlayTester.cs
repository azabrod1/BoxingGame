using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boxing.FighterRating;
using FightSim;
using log4net;
using Main;

namespace Utilities
{
    public class FightSimPlayTester
    {
        static readonly ILog LOGGER =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FighterCache Fighters;
        private readonly IFightSimulator FightSim = new FightSimulatorGauss();
        double MatchCoef { get; set; } = 0.05; //Higher values mean more likely fighters someone as good as them
        IFighterRating Rating;
        List<IFighterRating> AllRatings = new List<IFighterRating>();

        public FightSimPlayTester()
        {
            this.Fighters = new FighterCache();
            this.Rating = new EloFighterRating();
        }

        public void SimFights(int numFights)
        {

            for (int fightNight = 0; fightNight < numFights; ++fightNight)
            {
                LOGGER.Debug($"Fight Night {fightNight}");
                List<Fight> schedule = new List<Fight>();
                foreach (WeightClass wc in WeightClass.AllWeightClasses())
                    schedule.AddRange(ScheduleFights(wc));

                var outcomes = FightSim.SimulateManyFights(schedule);
                foreach (var outcome in outcomes)
                {
                    outcome.Fighters[0].UpdateRecord(outcome);
                    outcome.Fighters[1].UpdateRecord(outcome);
                    Rating.CalculateRatingChange(outcome);
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

            fighters.Sort((f1, f2) => Rating.Rating(f1).CompareTo(Rating.Rating(f2)));

            for (int f = 0; f < top; ++f)
            {
                Fighter curr = fighters[f];
                sb.AppendFormat($"{f + 1}. {curr.Name}\n\t{curr.Record} Rating: {Rating.Rating(curr)}\n");
                sb.AppendFormat($"\tSkill Level: {curr.OverallSkill()} Weight: {curr.Weight}");
                //sb.AppendLine();
                ///sb.AppendLine(curr.ToString());
                sb.AppendLine();

            }

            return sb.ToString();
        }

        private List<Fight> ScheduleFights(WeightClass wc)
        {
            List<Fighter> fighters = Fighters.AllFighters().Where(fighter => fighter.Weight == wc.Weight).ToList();
            List<Fight> schedule = new List<Fight>();

            //Sort in reverse order
            fighters.Sort((f1, f2) => Rating.Rating(f2).CompareTo(Rating.Rating(f1)));

            while (fighters.Count > 1)
            {
                int f1 = 0, f2 = 1;
                for (; f2 < fighters.Count - 1; ++f2)
                    if (MathUtils.RangeUniform(0, 1) < MatchCoef)
                        break;

                schedule.Add(new Fight(fighters[f1], fighters[f2]));
                fighters.RemoveAt(f1); fighters.RemoveAt(f2 - 1);
            }

            return schedule;
        }

        public List<Fighter> AddFighters(int numFighters, int weight)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            List<Fighter> added = new List<Fighter>();
            for (int f = 0; f < numFighters; ++f)
                added.Add(Fighters.CreateRandomFighter(weight));

            Rating.AddFighters(added);

            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;

            LOGGER.Info($"Generating {numFighters} fighters took {elapsed_time} ms");

            return added;
        }

    }
}
