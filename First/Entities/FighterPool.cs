using System;
using System.Collections.Generic;
using System.Linq;
using FightSim;

namespace Main
{
    public class FighterPool
    {
        public List<Fighter> Fighters { get; set; }
        Random rand; //todo check out my MathUtiliy lib from rand functionality


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

        public (double avg, double std) Stats()
        {
            double count = Fighters.Count();
            double avg = Fighters.Sum(d => d.Rank) / count;
            double std = Fighters.Sum(d => (d.Rank - avg) * (d.Rank - avg));
            std = Math.Sqrt(std / count);
            return (avg, std);   
        }

        public int Index(Fighter f)
        {
            return Index(f.Name);
        }

        public int Index(string name)
        {
            int index = Fighters.FindIndex(x => x.Name.Equals(name));
            return index;
        }


        public bool isTopFighter(Fighter f, int top = 30)
        {
            return isTopFighter(f.Name, top);
        }

        public bool isTopFighter(string name, int top = 30)
        {
            int index = this.Index(name);

            return (index >= 0 && index < top);
                
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
