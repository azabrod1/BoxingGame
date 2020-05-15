using System;
using System.Collections.Generic;
using System.Linq;
using FightSim;

namespace Main
{
    public class FighterPool
    {
        private List<Fighter> fighters;
        Random rand; //todo check out my MathUtility lib from rand functionality

        public FighterPool(int size = 1024)
        {
            fighters = new List<Fighter>(size);
            Fighter f;
            rand = new Random(Guid.NewGuid().GetHashCode());

            // assign random name and elo
            for (int i = 0; i < size; i++)
            {
                f = new Fighter("Fighter " + i.ToString());
                f.Rank = rand.NextDouble() * EloFightSimulator.ELO_MAX_INIT;
                fighters.Add(f);
            }

            fighters.Sort((x, y) => x.Rank.CompareTo(y.Rank));
            fighters.Reverse();

        }

        public (double avg, double std) Stats()
        {
            double count = fighters.Count();
            double avg = fighters.Sum(d => d.Rank) / count;
            double std = fighters.Sum(d => (d.Rank - avg) * (d.Rank - avg));
            std = Math.Sqrt(std / count);
            return (avg, std);   
        }

        public void SimulateFights(int epsilon = 16)
        {
            int op;
            int coeff;
            Fight fight;
            FightSimulator fs = new EloFightSimulator();
            for (int i = 0; i < fighters.Count(); i++)
            {
                coeff = (rand.Next(0, 2) > 0) ? 1 : -1;
                op = i + coeff * rand.Next(1, epsilon);
                if (op < 0)
                {
                    op += epsilon;
                }
                else if (op > fighters.Count() - 1)
                {
                    op -= epsilon;
                }
                fight = new Fight(fighters[i], fighters[op]);

                FightOutcome fo = fs.SimulateFight(fight);

                // this is for debug only - PLEASE REMOVE
                Console.WriteLine(fo.Winner.Name + " is the winner of " + fighters[i].Name + "(" + fighters[i].Rank.ToString("0") + ") vs " + fighters[op].Name + "(" + fighters[op].Rank.ToString("0") + ")");


            }



        }


    }
}
