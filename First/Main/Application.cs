using System;
using System.Collections.Generic;
using System.Linq;
using Boxing.FighterRating;
using DataStructures;
using FightSim;
using log4net;
using Newtonsoft.Json;

namespace Main
{
    [Serializable]
    [NewGame]
    public class Application
    {
        static readonly ILog LOGGER =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonPropertyAttribute] private GameData Data               { get;  set; }
        [JsonPropertyAttribute] private FighterCache Fighters       { get;  set; }
        [JsonPropertyAttribute] private IFighterRating Rating       { get;  set; }
        [JsonPropertyAttribute] private IFightSimulator FightSim    { get;  set; }
        [JsonPropertyAttribute] private FightSchedule FightSchedule { get;  set; }

        public Application()
        {
            FightSim = new FightSimulatorGauss();
        }

        public void InitNewGame()
        {
            Rating = new EloFighterRating(35);
        }

        public void NewGame()
        {
            //Create initial pool of fighters
            foreach (WeightClass wc in WeightClass.AllWeightClasses())
                for (int f = 0; f < wc.Size; ++f)
                {
                    RegisterFighter(Fighters.CreateRandomFighter(wc.Weight));
                }

            SimFights(50);
            Console.WriteLine(Status());
        }

        //TODO crap to remove 
        private void SimFights(int numFights)
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
                    //  FighterPopularity.UpdatePopularity(outcome);
                }
            }
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
                    if (MathUtils.RangeUniform(0, 1) < 0.05)
                        break;

                schedule.Add(new Fight(fighters[f1], fighters[f2]));
                fighters.RemoveAt(f1); fighters.RemoveAt(f2 - 1);
            }

            return schedule;
        }

        public string Status(int top = -1)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
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
               // sb.AppendLine(curr.ToString());
                sb.AppendLine();

            }

            return sb.ToString();
        }

        public void RegisterFighter(Fighter fighter)
        {
            Rating.AddFighter(fighter);
        }


    }
}
