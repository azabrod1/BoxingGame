using System;
using System.Collections.Generic;
using System.Linq;
using FightSim;

namespace Main
{
    public class FighterPool
    {
        public readonly int top_index = 30;


        public List<Fighter> Fighters { get; set; }
        Random rand; //todo check out my MathUtiliy lib from rand functionality


        public FighterPool(int size = 1024, int top_index = 30)
        {
            Fighters = new List<Fighter>(size);
            Fighter f;
            rand = new Random(Guid.NewGuid().GetHashCode());

            // assign random name and elo
            for (int i = 0; i < size; i++)
            {
                f = new Fighter("Fighter " + i.ToString());
                f.Record.Rank = rand.NextDouble() * EloFightSimulator.ELO_MAX_INIT;
                Fighters.Add(f);
            }

            Fighters.Sort((x, y) => x.Record.Rank.CompareTo(y.Record.Rank));
            Fighters.Reverse();

        }

        public (double avg, double std) Stats()
        {
            double count = Fighters.Count();
            double avg = Fighters.Sum(d => d.Record.Rank) / count;
            double std = Fighters.Sum(d => (d.Record.Rank - avg) * (d.Record.Rank - avg));
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


        public bool isTopFighter(Fighter f)
        {
            return isTopFighter(f.Name);
        }

        public bool isTopFighter(string name)
        {
            int index = this.Index(name);

            return isTopFighter(index);
        }

        public bool isTopFighter(int index)
        {
            return (index >= 0 && index < top_index);
        }


        public FightOutcome SimulateFight(int index1, int index2)
        {
            Fighter f1 = Fighters[index1];
            Fighter f2 = Fighters[index2];
            Fight ff = new Fight(f1, f2);
            FightSimulator fs = new EloFightSimulator();
            FightOutcome fo = fs.SimulateFight(ff);

            if (this.isTopFighter(index1) || this.isTopFighter(index2))
            {
                Console.WriteLine(fo.Winner.Name + " wins " + Fighters[index1].Name + "(" + Fighters[index1].Record.Rank.ToString("0") + ") vs " + Fighters[index2].Name + "(" + Fighters[index2].Record.Rank.ToString("0") + ")");

            }
            return fo;

        }

        public void SimulateFights(int epsilon = 16)
        {
            int op;
            int coeff;
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

                FightOutcome fo = this.SimulateFight(i, op);



            }



        }


    }
}
