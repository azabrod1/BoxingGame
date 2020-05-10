using System;
using System.Collections.Generic;
using System.Linq;
namespace Main
{
    public class FighterPool
    {
        private List<Fighter> fighters;
        Random rand; 


        public FighterPool(int size = 1024)
        {
            fighters = new List<Fighter>(size);
            Fighter f;
            rand = new Random(Guid.NewGuid().GetHashCode());

            // assign random name and elo
            for (int i = 0; i < size; i++)
            {
                double elo = rand.NextDouble() * 1000.0;
                f = new Fighter("Fighter " + i.ToString(), elo);
                fighters.Add(f);
            }

            fighters.Sort((x, y) => x.elo.CompareTo(y.elo));
            fighters.Reverse();

        }

        public (double avg, double std) stats()
        {
            double count = fighters.Count();
            double avg = fighters.Sum(d => d.elo) / count;
            double std = fighters.Sum(d => (d.elo - avg) * (d.elo - avg));
            std = Math.Sqrt(std / count);
            return (avg, std);   
        }

        public void simulateFights(int epsilon = 16)
        {
            int op;
            int coeff;
            Fight fight;
            FightSimulator fs = new FightSimulator();
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

                fs.simulate(fight);

                Console.WriteLine(fight.result.ToString() + ": " + fighters[i].name + "(" + fighters[i].elo.ToString("0") + ") vs " + fighters[op].name + "(" + fighters[op].elo.ToString("0") + ")");


            }



        }


    }
}
