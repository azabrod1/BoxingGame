using System;
using System.Collections.Generic;
using Main;

namespace FightSim
{
    public interface IScheduler
    {
        public IEnumerable<Fight> ScheduleFights(List<Fighter> fighters);
     
    }
}
