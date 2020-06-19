using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;
using System.Collections.Generic;
using FightSim;

namespace FighterRanking
{


    public static class FighterPopularity
    {
        // simple dictionaries to hold country and weight popularity coefficients
        // #todo: integrate with the rest of the program and persist the coefficients in XLM

        //private static Dictionary<string, double> CountryCoefficient = new Dictionary<string, double>() { { "US", 1.1 }, { "Mexico", 2.0 } };

        //private static Dictionary<int, double> WeightCoefficient = new Dictionary<int, double>() { { 108, 1.0 }, { 147, 1.1 }, { 154, 2.0 } };

        //private static Dictionary<double, List<int>> WeightCoefficient = new Dictionary<int, double>() { { 1.0,  }, { 147, 1.1 }, { 154, 2.0 } };


        // PUBLIC INTERFACE


        /// <summary>
        //  This method initializes Popularity metrics for a fighter f and updates it 
        //  between the fights. Thus the only input is the fighter object itself
        //
        //  NB: to update popularity as a result of a fight, use UpdatePopularity(FighterOutcome method)
        //
        //  NB: Please note that Elo metrics is being set by EloFighterRanking.CalculateRatingChange() method
        //  #todo: make sure the implementation is consistent between Elo and other metrics
        //
        /// </summary>
        public static void UpdatePopularity(Fighter f)
        {
            double fans;
            double casuals;
            double followers;


            if (!f.Performance.ContainsKey("Active"))
            {
                // first time the method is called for a new fighter
                // need to populate initial values
                //(Active is an indicator the fighter is initialized)
                f.Performance["Active"] = 1;

                // todo need to make proper values

                f.Performance["Fans"] = 1000;
                f.Performance["Casuals"] = 1000;
                f.Performance["Followers"] = 1000;
                f.Performance["Elo"] = 500;

            }
            else
            {

                fans = f.Performance["Fans"];
                casuals = f.Performance["Casuals"];
                followers = f.Performance["followers"];

                // do the calculatiuon
                if (fans == 0)
                    fans = 1000;


                fans = fans + 1;
                // end of calculation


                f.Performance["Fans"] = fans;
            }
        }


        /// <summary>
        //  The method update fighter popularity of both fighters as a result
        //  of FightOutcome object
        //
        //  at the end of the method, FighterOutcome viewership is set by the 
        //  result of a private FightViewers(FighterOutcome) method. No need to 
        //  to call FightViewers outside of this class.
        /// </summary>
        public static void UpdatePopularity(FightOutcome fo)
        {

            double fans1 = fo.Fighter1().Performance["Fans"];
            double fans2 = fo.Fighter2().Performance["Fans"];

            double casuals1 = fo.Fighter1().Performance["Casuals"];
            double casuals2 = fo.Fighter2().Performance["Casuals"];

            double followers1 = fo.Fighter1().Performance["Followers"];
            double followers2 = fo.Fighter2().Performance["Followers"];


            Console.WriteLine("Fighter1: " + ToString(fo.Fighter1()));
            Console.WriteLine("Fighter2: " + ToString(fo.Fighter2()));


            // do the calculation

            if (fo.Fighter1() == fo.Winner)
            {

                //winner


                double delta = 0.09 * casuals1 * (1 - PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                fans1 += delta;
                casuals1 -= delta;

                delta = 0.4 * fo.Interested * (1 - PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                casuals1 += delta;
                //fo.Interested =- delta;

                delta = 0.09 * casuals1 * (1 - PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                followers1 += delta;
                casuals1 -= 0.18 * casuals1;

                //loser

                delta = 0.1 * fans2 * (1 - PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                fans2 -= delta;
                casuals2 += delta;

                delta = 0.1 * casuals2 * (1 - PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                casuals2 -= delta;
                //fo.Interested = +delta;

                delta = 0.1 * followers2 * (1 - PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                followers2 -= delta;
                casuals2 += delta;

            }
            else if (fo.Fighter2() == fo.Winner)
            {

                double delta = 0.09 * casuals1 * (PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                fans1 -= delta;
                casuals1 += delta;

                delta = 0.4 * fo.Interested * (PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                casuals1 -= delta;
                //fo.Interested =+ delta;

                delta = 0.09 * casuals1 * (PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                followers1 -= delta;
                casuals1 += 0.18 * casuals1;

                //winner

                delta = 0.1 * fans2 * (PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                fans2 += delta;
                casuals2 -= -delta;

                delta = 0.1 * casuals2 * (PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                casuals2 += delta;
                //fo.Interested =- delta;

                delta = 0.1 * followers2 * (PWin(fo));
                delta = MathUtils.Gauss(delta, 0.5);
                followers2 += delta;
                casuals2 -= delta;


            }


            // end of calculation

            fo.Fighter1().Performance["Fans"] = fans1;
            fo.Fighter2().Performance["Fans"] = fans2;
            fo.Fighter1().Performance["Casuals"] = casuals1;
            fo.Fighter2().Performance["Casuals"] = casuals2;
            fo.Fighter1().Performance["Followers"] = followers1;
            fo.Fighter2().Performance["Followers"] = followers2;

            fo.Viewership = FightViewers(fo);

            Console.WriteLine(ToString(fo));
            Console.WriteLine("Fighter1: " + ToString(fo.Fighter1()));
            Console.WriteLine("Fighter2: " + ToString(fo.Fighter2()) + "\n");



        }

        public static string ToString(Fighter f)
        {

            var p = f.Performance;
            string s = $"Fighter {f.Name}:Fans={p["Fans"]:N0},Followers={p["Followers"]:N0}," +
                $"Casuals={p["Casuals"]:N0},Elo={p["Elo"]:N0}";

            return s;
        }

        public static string ToString(FightOutcome fo)
        {
            string s = ($"Viewership:{fo.Viewership:N0}");

            return s;
        }


        // PRIVATE METHODS

        private static double FightViewers(FightOutcome fo)
        {
            double viewers;


            Fighter f1 = fo.Fighter1();
            Fighter f2 = fo.Fighter2();


            //Country country = f1.Nationality;
            //Console.WriteLine(f1.Nationality + "'s buff is " + country.PopularityBuff);

            WeightClass w1 = (WeightClass)f1.Weight;

            //WeightCoefficient[w1.Weight]



            fo.Interested = MathUtils.Gauss(((f1.Performance["Elo"] + f2.Performance["Elo"]) / 2), 100) *
                (f1.Nationality.PopularityBuff * f2.Nationality.PopularityBuff * w1.Popularity + f1.Belts + f2.Belts);



            //viewers = f1.Performance["Fans"] + f1.Performance["Followers"] + f1.Performance["Casuals"];
            //viewers += (f1.Performance["Fans"] + f2.Performance["Followers"] + f2.Performance["Casuals"]);


            viewers = fo.Interested + (PWin(fo)) * f1.Performance["Followers"] + (1 - PWin(fo) * f2.Performance["Followers"]) + (PWin(fo)) * f1.Performance["Fans"] + (1 - PWin(fo) * f2.Performance["Fans"]);


            return viewers;



        }


        private static double PWin(FightOutcome fo)
        {
            return 1 / (1 + Math.Pow(10, (fo.Fighter2().Performance["Elo"] - fo.Fighter1().Performance["Elo"]) / 400));

        }


        



    }


}