using System.Collections.Generic;
using System.Linq;
using System.Text;
using Main;

namespace FightSim
{
    //This exists to make testing easier and collecting data on simulations
    public static class FightStatsUtility
    {
        public static bool IsKO(this FightSim.FightOutcome outcome)
        {
            return outcome.Method == FightSim.MethodOfResult.KO || outcome.Method == FightSim.MethodOfResult.TKO;
        }

        public static double WinPercent(this List<FightSim.FightOutcome> fights, Main.Fighter fighter, bool byKo = false)
        {
            return (double) 100 * fights.Wins(fighter, byKo) / fights.Count;
        }

        public static double DrawPercent(this List<FightSim.FightOutcome> fights)
        {
            return WinPercent(fights, null);
        }

        public static int Wins(this List<FightSim.FightOutcome> fights, Main.Fighter fighter, bool byKo = false)
        {
            return fights.Where(outcome => outcome.Winner == fighter && (!byKo || outcome.IsKO() )).Count();
        }

        public static int Draws(this List<FightSim.FightOutcome> fights)
        {
            return fights.Where(outcome => outcome.Winner == null).Count();
        }

        public static int KOs(this List<FightSim.FightOutcome> fights, Main.Fighter fighter)
        {
            return fights.Wins(fighter, true);
        }

        public static double PercentWinsByKO(this List<FightSim.FightOutcome> fights, Main.Fighter fighter)
        {
            return 100d * fights.Wins(fighter, true) / fights.Wins(fighter, false);
        }

        public static string SummaryFightOutcomes(this List<FightSim.FightOutcome> fights)
        {
            StringBuilder sb = new StringBuilder();

            List<Main.Fighter> winners = fights.Select(fight => fight.Winner).Distinct().ToList();

            foreach(var fighter in winners){
                if(fighter != null)
                    sb.AppendFormat($"--{fighter.Name}--\n" )
                      .AppendFormat("Wins {0} ({1}%)\n", fights.Wins(fighter), fights.WinPercent(fighter))
                      .AppendFormat("KOs  {0} ({1}%)\n", fights.Wins(fighter, true), fights.PercentWinsByKO(fighter));
            }

            sb.AppendFormat($"--Draws--\nCount {fights.Draws() } ({fights.DrawPercent()}%)\n");

            Dictionary<MethodOfResult, int> methodsOfResult = fights.MethodOfResultStats();

            sb.AppendLine("Methods Of Result:");

            foreach (KeyValuePair<MethodOfResult, int> entry in methodsOfResult)
            {
                sb.AppendFormat($"{entry.Key} {entry.Value}\n");
            }

            return sb.ToString();
        }

        public static Dictionary<MethodOfResult, int> MethodOfResultStats(this List<FightSim.FightOutcome> fights)
        {
            return fights.GroupBy(fight => fight.Method).ToDictionary(x => x.Key, x => x.Count());
        }

        public static string SummaryStats(this List<FightStats> list, string f1Name = "Red Corner", string f2Name = "Blue Corner")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Stats for {0} elements\n", list.Count);

            string[] fighters = new string[] { f1Name, f2Name };

            foreach (string fighter in fighters) //todo: CAN SOMEONE SHOW ME LESS UGLY WAY TO LOOP TWICE! This is dog shit!
            {
                bool isFighter1 = fighter == fighters[0];
                sb.AppendFormat("{0}\n", fighter)
                  .AppendFormat("Avg Damage     {0}\n", list.AverageDamage(isFighter1))
                  .AppendFormat("Avg Thrown     {0}\n", list.AverageThrown(isFighter1))
                  .AppendFormat("Avg Landed     {0}\n", list.AverageLanded(isFighter1))
                  .AppendFormat("Avg Accuracy   {0}\n", list.LandedPercent(isFighter1))
                  .AppendFormat("Jab Percent    {0}\n", list.AvgJabPercent(isFighter1))
                  .AppendFormat("Avg Knockdowns {0}\n", list.KnockDownsAvg(isFighter1))
                  .AppendLine();
            }

            return sb.ToString();
        }

        public static double AverageDamage(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Damage.Fighter1 : s.Damage.Fighter2).Average();
        }

        public static double StandardDeviationDamage(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Damage.Fighter1 : s.Damage.Fighter2).ToList().StandardDeviation();
        }

        public static int TotalLanded(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Landed.Fighter1 : s.Landed.Fighter2).Sum();
        }

        public static int TotalThrown(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Thrown.Fighter1 : s.Thrown.Fighter2).Sum();
        }

        public static double AverageLanded(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Landed.Fighter1 : s.Landed.Fighter2).Average();
        }

        public static double AverageThrown(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Thrown.Fighter1 : s.Thrown.Fighter2).Average();
        }

        public static double LandedPercent(this List<FightStats> list, bool Fighter1)
        {
            return (double)100 * TotalLanded(list, Fighter1) / TotalThrown(list, Fighter1);
        }

        public static double AvgJabPercent(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(stats => stats.JabPercent(Fighter1)).Average();
        }

        public static int KnockDowns(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Knockdowns.Fighter1 : s.Knockdowns.Fighter2).Sum();
        }

        public static double KnockDownsAvg(this List<FightStats> list, bool Fighter1)
        {
            return list.Select(s => Fighter1 ? s.Knockdowns.Fighter1 : s.Knockdowns.Fighter2).Average();
        }


        public static FightStats Condense(this List<FightStats> list, bool useAverages = false)
        {
            FightStats summaryOfSummaries = new FightStats();
            foreach (FightStats s in list)
                summaryOfSummaries.Append(s);

            if (useAverages) //Flawed due to integer rounding issues, used mostly for rounds - for testing only
            {
                summaryOfSummaries.Damage.Fighter1 /= list.Count;
                summaryOfSummaries.Damage.Fighter2 /= list.Count;

                summaryOfSummaries.Thrown.Fighter1 /= list.Count;
                summaryOfSummaries.Thrown.Fighter2 /= list.Count;

                summaryOfSummaries.Landed.Fighter1 /= list.Count;
                summaryOfSummaries.Landed.Fighter2 /= list.Count;

                summaryOfSummaries.Jabs.Fighter1 /= list.Count;
                summaryOfSummaries.Jabs.Fighter2 /= list.Count;
            }

            return summaryOfSummaries;

        }

    }
}
