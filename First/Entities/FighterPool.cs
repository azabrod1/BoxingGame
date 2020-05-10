using System;
using System.Collections.Generic;
namespace Main
{
    public class FighterPool
    {
        private List<Fighter> fighters;



        public FighterPool(int size = 1024)
        {
            fighters = new List<Fighter>(size);
            Fighter f;
            Random rand = new Random(Guid.NewGuid().GetHashCode());
               
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





    }
}
