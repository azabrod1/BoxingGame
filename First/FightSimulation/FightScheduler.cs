using System;
using System.Collections.Generic;
using Main;

namespace Boxing.FightSimulation
{
    public class FightScheduler
    {
        public FightScheduler()
        {
        }

        //public IEnumerable<Fight> ScheduleFights(List<Fighter> fighters)
        //{


        //}


        bool WillingToFight(Fighter a, Fighter b)
        {
            if (a.Age + a.AgeCurve < 26)
                return false;

            return true;
        }

    }
}
