using System;
using Main;
namespace Boxing.FighterRanking

{
    public class FightPopularity
    {
        private FighterPopularity Frp;
        public FightPopularity(FighterPopularity frp) //, EloFighterRating efr)
        {
            Frp = frp;
        }

       public double GetViewers(Fighter f1, Fighter f2)
       {

            var popularity1 = Frp.PopularityData(f1);
            var popularity2 = Frp.PopularityData(f2);

            Console.WriteLine(popularity1.fans);



            return 0;
       }


    }
}
