using System;
using System.Collections.Generic;
using System.Linq;
using FightSim;

namespace Main
{
    public class FighterPool
    {
        public List<Fighter> Fighters { get; set; }

        readonly Random rand; 

        public FighterPool(int size = 1024)
        {
            Fighters = new List<Fighter>(size);
            Fighter f;
            rand = new Random(Guid.NewGuid().GetHashCode());

            // assign random name and elo
            for (int i = 0; i < size; i++)
            {
                f = new Fighter("Fighter " + i.ToString());
                f.Rank = rand.NextDouble() * EloFightSimulator.ELO_MAX_INIT;
                Fighters.Add(f);
            }

            Fighters.Sort((x, y) => x.Rank.CompareTo(y.Rank));
            Fighters.Reverse();

        }

        public List<Fighter> TopFighters(int n)
        {
            if (n >= this.Fighters.Count)
            {
                return this.Fighters.ToList();
            }
            return Fighters.GetRange(0, n);

        }


        


        public (double avg, double std) Stats()
        {
            double count = Fighters.Count();
            double avg = Fighters.Sum(d => d.Rank) / count;
            double std = Fighters.Sum(d => (d.Rank - avg) * (d.Rank - avg));
            std = Math.Sqrt(std / count);
            return (avg, std);   
        }

        public void SimulateFights(int epsilon = 16)
        {
            int op;
            int coeff;
            Fight fight;
            FightSimulator fs = new EloFightSimulator();
            for (int i = 0; i < Fighters.Count(); i++)
            {
                coeff = (rand.Next(0, 2) > 0) ? 1 : -1;
                op = i + coeff * rand.Next(1, epsilon);
                if (op < 0)
                {
                    op += epsilon;
                }
                else if (op > Fighters.Count() - 1)
                {
                    op -= epsilon;
                }
                fight = new Fight(Fighters[i], Fighters[op]);

                FightOutcome fo = fs.SimulateFight(fight);

                // this is for debug only - PLEASE REMOVE
                Console.WriteLine(fo.Winner.Name + " is the winner of " + Fighters[i].Name + "(" + Fighters[i].Rank.ToString("0") + ") vs " + Fighters[op].Name + "(" + Fighters[op].Rank.ToString("0") + ")");


            }



        }


    }
}
