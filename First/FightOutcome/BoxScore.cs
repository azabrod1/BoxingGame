using System;
using System.Collections.Generic;
using System.Linq;

namespace Fighting
{
    public class BoxScore
    {
        public int Result; //Minute the fight ended
        public List<double> Damage;
        public List<double> PunchesLanded;
        public List<double> PunchesThrown;

        public BoxScore()
        {
            this.Result = -1;
            this.Damage = new List<double>();
            this.PunchesLanded = new List<double>();
            this.PunchesThrown = new List<double>();

        }

        public double LandedPercent()
        {
            return PunchesLanded.Sum() / PunchesThrown.Sum();

        }

        public double TotalDamage()
        {
            return Damage.Sum();
        }

        public double AvgDamage()
        {
            return Damage.Average();
        }

        public void AppendData(int round, double damage, double punchesThrown, double punchesLanded){
            while(Damage.Count() <= round)
            {
                Damage.Add(0);
                PunchesLanded.Add(0);
                PunchesThrown.Add(0);
            }

            Damage[round] += damage;
            PunchesThrown[round] += punchesThrown;
            PunchesLanded[round] += punchesLanded;
        }
       
        public override string ToString()
        {
            return String.Format($"Result: {Result} Landed % {LandedPercent()}, Avg Damage: {AvgDamage()}");
        }
    }
}
