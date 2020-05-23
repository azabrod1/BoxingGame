using System;
using System.Collections.Generic;
using System.Linq;
using FightSim;

namespace Main
{
    public class FighterPool
    {
        public List<Fighter> Fighters { get; set; }
        private Random rand;
        public readonly int top_index;

        public FighterPool(int size = 1024, int top_index = 30)
        {
            Fighters = new List<Fighter>(size);
            Fighter f;
            rand = new Random(Guid.NewGuid().GetHashCode());
            this.top_index = top_index;

            // assign random name and elo
            for (int i = 0; i < size; i++)
            {
                f = new Fighter("Fighter " + i.ToString());
             
                f.Record.Rank = 50;
                Fighters.Add(f);
            }

            SortFighters();

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


        public bool IsTopFighter(Fighter f)
        {
            return IsTopFighter(f.Name);
        }

        public bool IsTopFighter(string name)
        {
            int index = this.Index(name);

            return IsTopFighter(index);
        }

        public bool IsTopFighter(int index)
        {
            return (index >= 0 && index < top_index);
        }

        public FightOutcome SimulateFight(int index1, int index2, bool printResult = true, bool sortFighters = false)
        {
            Fighter f1 = Fighters[index1];
            Fighter f2 = Fighters[index2];
            Fight ff = new Fight(f1, f2);
            FightSimulator fs = new EloFightSimulator();
            FightOutcome fo = fs.SimulateFight(ff);

            if (printResult && (this.IsTopFighter(index1) || this.IsTopFighter(index2)))
            {
                PrintFightResult(index1, index2, fo);
            }
            if (sortFighters) // sort the pool 
            {
                SortFighters();
            }
            return fo;

        }

        public void PrintFightResult(int index1, int index2, FightOutcome fo)
        {
            Console.WriteLine(Fighters[index1].Name + "(elo = " + Fighters[index1].Record.Rank.ToString("0") + ") vs " + Fighters[index2].Name + "(elo = " + Fighters[index2].Record.Rank.ToString("0") + ")");

            Console.WriteLine(fo.Winner.Name + " wins in front of " + fo.Viewership + "k audience");

            Console.Write("Elo changes: " + Fighters[index1].Name + " (" + Fighters[index1].Record.PreviousRank.ToString("0") + " to " + Fighters[index1].Record.Rank.ToString("0") + ") ");

            Console.WriteLine(Fighters[index2].Name + " (" + Fighters[index2].Record.PreviousRank.ToString("0") + " to " + Fighters[index2].Record.Rank.ToString("0") + ")\n");

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
            SortFighters();


        }

        public void SortFighters()
        {
            Fighters.Sort((x, y) => x.Record.Rank.CompareTo(y.Record.Rank));
           // Fighters.Reverse();
        }
    }

    

}
