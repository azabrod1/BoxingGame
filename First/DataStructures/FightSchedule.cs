using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;
using FightList = System.Collections.Concurrent.ConcurrentDictionary<Main.Fight, bool>;

namespace DataStructures
{
    [Serializable]
    [NewGame]
    public class FightSchedule
    {
        [JsonProperty] private ConcurrentDictionary<DateTime, FightList> Schedule  { get; set; }
        [JsonProperty] private ConcurrentDictionary<Fighter, Fight>      NextFight { get; set; }
        [JsonProperty] private ConcurrentDictionary<Fighter, Fight>      LastFight { get; set; }

        public void AddFight(Fight fight)
        {
            FightList fightsThatDay = Schedule.GetOrAdd(fight.Date, key => new FightList());
            fightsThatDay[fight] = true;
            NextFight[fight.Fighter0()] = fight;
            NextFight[fight.Fighter1()] = fight;
        }

        public bool CancelFight(Fight fight)
        {
            if (!Schedule.TryGetValue(fight.Date, out FightList fightsThatDay))
                return false;

            return fightsThatDay.TryRemove(fight, out _);
        }

        public bool CompleteFight(Fight fight)
        {
            if(Schedule.TryGetValue(fight.Date, out FightList fightsThatDay) && fightsThatDay.TryRemove(fight, out _))
            {
                LastFight[fight.Fighter0()] = fight;
                LastFight[fight.Fighter1()] = fight;
                return true;
            }

            return false;
        }

        public System.Collections.Generic.ICollection<Fight> FightsOn(DateTime date)
        {
            if (Schedule.TryGetValue(date, out FightList fightsThatDay))
                return fightsThatDay.Keys;

            return null;
        }

        //Is this fight still on the schedule?
        public bool IsScheduled(Fight fight)
        {
            if (!Schedule.TryGetValue(fight.Date, out FightList fightsThatDay))
                return false;

            return fightsThatDay.ContainsKey(fight);
        }

        public Fight GetNextFight(Fighter fighter)
        {
            if (!NextFight.TryGetValue(fighter, out Fight fight) || !IsScheduled(fight))
                return null;

            return fight;
        }

        public Fight GetLastFight(Fighter fighter)
        {
            if (!NextFight.TryGetValue(fighter, out Fight fight) || !IsScheduled(fight))
                return null;

            return fight;
        }

    }
}
