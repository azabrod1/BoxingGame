using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace FightSim
{
    public interface FightSimulator
    {
        public FightOutcome SimulateFight(Main.Fight fight);

        //Same as above but with round by roud summary
        public (FightOutcome outcome, List<FightStats> Stats) SimulateFightWithDetails(Main.Fight fight)
        {
            throw new NotImplementedException(); 
        }

        /* Concurrency */

        const int FIGHTS_PER_THREAD = 500; //This will one day be a bigger #

        public  List<FightOutcome> SimulateManyFights(List<Main.Fight> fights)
        {
            FightOutcome[] results = new FightOutcome[fights.Count];

            List<Task> tasks = new List<Task>();

            for (int f = 0; f < fights.Count; f += FIGHTS_PER_THREAD)
            {
                int idx = f;
                tasks.Add(Task.Factory.StartNew(() =>
                                 SimFights(results, fights, idx, Math.Min(fights.Count, idx + FIGHTS_PER_THREAD) - 1)));
            }

            foreach (Task task in tasks)
                task.Wait();

            return new List<FightOutcome>(results);
        }

        private void SimFights(FightOutcome[] results, List<Main.Fight> fights, int from, int to)
        {
            for (int t = from; t <= to; t++)
                results[t] = SimulateFight(fights[t]);
        }

        public List<(FightOutcome outcome, List<FightStats> Stats)> SimulateManyFightsWithDetails(List<Main.Fight> fights)
        {
            (FightOutcome outcome, List<FightStats> Stats)[] results = new (FightOutcome outcome, List<FightStats> Stats)[fights.Count];

            List<Task> tasks = new List<Task>();

            for (int f = 0; f < fights.Count; f += FIGHTS_PER_THREAD)
            {
                int idx = f;
                tasks.Add(Task.Factory.StartNew(() =>
                                 SimFightsWithDetails(results, fights, idx, Math.Min(fights.Count, idx + FIGHTS_PER_THREAD) - 1) )); 
            }

            foreach (Task task in tasks)
                task.Wait();

            return new List<(FightOutcome outcome, List<FightStats> Stats)>(results);
        }

        private void SimFightsWithDetails((FightOutcome outcome, List<FightStats> Stats)[] results, List<Main.Fight> fights, int from, int to)
        {
            for (int t = from; t <= to; t++)
                results[t] = SimulateFightWithDetails(fights[t]);
        }

    }
}
