using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;
using System.Collections.Generic;
using FightSim;


 

namespace Boxing.FighterRanking
{
  

    public class FighterPopularity
    {
        // example of updating Fighter popularity metics (Fans) outside of a fight
        public static void UpdateFans(Fighter f)
        {
            double fans = f.Performance["Fans"];

            // do the calculatiuon
            if (fans == 0)
                fans = 1000;


            fans = fans + 1;
            // end of calculation


            f.Performance["Fans"] = fans;
        }

        // example of updating Fighter popularity metics (Fans) as a result of a fight

        public static void UpdateFans(FightOutcome fo)
        {

            double fans1 = fo.Fighter1().Performance["Fans"];
            double fans2 = fo.Fighter2().Performance["Fans"];



            // do the calculation

            if (fo.Fighter1() == fo.Winner)
            {
                fans1 *= 1.1;
                fans2 *= 0.9;

            }
            else if (fo.Fighter2() == fo.Winner)
            {
                fans1 *= 0.9;
                fans2 *= 1.1;
            }


            // end of calculation

            fo.Fighter1().Performance["Fans"] = fans1;
            fo.Fighter2().Performance["Fans"] = fans2;


        }


        public static double FightViewers(FightOutcome fo)
        {
            double viewers;
            Fighter f1 = fo.Fighter1();
            Fighter f2 = fo.Fighter2();

            viewers = f1.Performance["Fans"] + f1.Performance["Followers"] + f1.Performance["Casuals"];
            viewers += (f1.Performance["Fans"] + f2.Performance["Followers"] + f2.Performance["Casuals"]);

            return viewers;



        }






    }
}
