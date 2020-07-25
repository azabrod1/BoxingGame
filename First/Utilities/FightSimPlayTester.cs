using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boxing.FighterRating;
using FightSim;
using log4net;
using Main;
using MathNet.Numerics;

namespace Utilities
{
    [NewGame]
    public class FightSimPlayTester
    {
        static readonly ILog LOGGER =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FighterCache Fighters { get; set; }
        private readonly IFightSimulator FightSim = new FightSimulatorGauss();
        double MatchCoef { get; set; } //Higher values mean more likely fighters someone as good as them
        IFighterRating Rating { get; set; }
        List<IFighterRating> AllRatings = new List<IFighterRating>();

        static List<double> Xs = new List<double>();
        static List<double> Ys = new List<double>();

        public static Dictionary<int, (int fights, int wins)> stat = new Dictionary<int, (int fights, int wins)>();


        public FightSimPlayTester(double coef = 0.05)
        {
            this.Fighters = new FighterCache();
            this.Rating = new EloFighterRating();
            this.MatchCoef = coef;
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
                    AssessOutcome(outcome);
                    Rating.CalculateRatingChange(outcome);
                    //  FighterPopularity.UpdatePopularity(outcome);
                }
            }
        }

        void AssessOutcome(FightOutcome outcome)
        {
             double f0E = Rating.Rating(outcome.Fighters[0]);
             double f1E = Rating.Rating(outcome.Fighters[1]);

           // double f0E = outcome.Fighters[0].OverallSkill();
           // double f1E = outcome.Fighters[1].OverallSkill();

            int skillDif = Convert.ToInt32( f0E - f1E);

            //if (skillDif < 0)
            //    return;

            if (!stat.ContainsKey(skillDif))
                stat[skillDif] = (0, 0);

            double winnerNum = outcome.WinnerNum();
            if (winnerNum == -1)
                winnerNum = 0.5;

            if (winnerNum == 0.5)
                return;

            Xs.Add(f1E - f0E);
            Ys.Add(winnerNum);

            (int fights, int wins) = stat[skillDif];

            stat[skillDif] = (fights + 1, wins + (int) winnerNum);

        }


        public double[] Regress()
        {

            double[] p = Fit.Polynomial(Xs.ToArray(), Ys.ToArray(), 2); // polynomial of order 2
                                                                        //   Tuple<double, double> p2 = Fit.Line(Xs.ToArray(), Ys.ToArray());
                                                                        //    Console.WriteLine(p2);


            double[][] samples = new double[Xs.Count][];

            for(int x = 0; x < Xs.Count; ++x)
                samples[x] = new double[] { Xs[x], Xs[x] * Math.Abs(Xs[x]) };
            
            double[] p2 = Fit.MultiDim(
                samples,
                Ys.ToArray(),
                intercept: true);


            var list = stat.Select(i => i).ToList();
            list.Sort((x, y) => y.Key.CompareTo(x.Key));

            list.Select(i => $"{i.Key}: {100.0*(double)i.Value.wins/ i.Value.fights}").ToList().ForEach(Console.WriteLine);

            double[] xs = new double[list.Count];
            double[] ys = new double[list.Count];

            for(int i = 0; i < list.Count; ++i)
            {
                xs[i] = list[i].Key;
                ys[i] = 100.0 * (double)list[i].Value.wins / list[i].Value.fights;
            }

            (var a, var b) = Fit.Line(xs, ys);

            Console.WriteLine($"a {a} b{b} R {GoodnessOfFit.RSquared(xs.Select(x => a + b * x), ys)}  ");

            Console.WriteLine(Utility.UsefulString(p) + "\n.........\n");

            return p2;
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
                sb.AppendLine(curr.ToString());
                sb.AppendLine();

            }

            return sb.ToString();
        }

    }
}
