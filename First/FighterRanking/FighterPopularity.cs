using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;
using System.Collections.Generic;
using FightSim;
using System.Text;
using System.Xml;

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

                f.Performance["Fans"] = 20;
                f.Performance["Casuals"] = 5;
                f.Performance["Followers"] = 5;
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

            double CountryCoeff1 = fo.Fighter1().Nationality.PopularityBuff;
            double CountryCoeff2 = fo.Fighter2().Nationality.PopularityBuff;
            double WeightCoeff = ((WeightClass)fo.Fighter1().Weight).Popularity;


            // do the calculation

            if (fo.Fighter1() == fo.Winner)
            {

                //winner


                double delta = 0.09 * casuals1 * (1 - PWin(fo)) * WeightCoeff * CountryCoeff1;
                delta = MathUtils.Gauss(delta, 0.5);
                fans1 += delta;
                casuals1 -= delta;

                delta = 0.4 * fo.Interested * (1 - PWin(fo)) * WeightCoeff * CountryCoeff1;
                delta = MathUtils.Gauss(delta, 0.5);
                casuals1 += delta;
                //fo.Interested =- delta;

                delta = 0.09 * casuals1 * (1 - PWin(fo)) * WeightCoeff * CountryCoeff1;
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

                //loser

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

                delta = 0.1 * fans2 * (PWin(fo)) * WeightCoeff * CountryCoeff2;
                delta = MathUtils.Gauss(delta, 0.5);
                fans2 += delta;
                casuals2 -= -delta;

                delta = 0.1 * casuals2 * (PWin(fo)) * WeightCoeff * CountryCoeff2;
                delta = MathUtils.Gauss(delta, 0.5);
                casuals2 += delta;
                //fo.Interested =- delta;

                delta = 0.1 * followers2 * (PWin(fo)) * WeightCoeff * CountryCoeff2;
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

        private static double getBase(Fighter F)
        {
            return F.Performance["Followers"] + F.Performance["Fans"];
        }

        private static double fightAttendance(Fight fight, Venue venue, double ticketPrice)

        {

            Fighter A = fight.Fighter1();
            Fighter B = fight.Fighter2();
            FightOutcome fo = fight.Outcome;

            double elasticity = venue.Elasticity;




            /*using (XmlReader reader = XmlReader.Create(@"venues.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {

                            case "Alabama":
                                elasticity = -6;
                                break;
                            case "Alaska":
                                elasticity = -6;
                                break;
                            case "Arizona":
                                elasticity = -6;
                                break;
                            case "Arkansas":
                                elasticity = -6;
                                break;
                            case "California":
                                elasticity = -3;
                                break;
                            case "Colorado":
                                elasticity = -6;
                                break;
                            case "Connecticut":
                                elasticity = -6;
                                break;
                            case "Delaware":
                                elasticity = -6;
                                break;
                            case "Florida":
                                elasticity = -5;
                                break;
                            case "Georgia":
                                elasticity = -5;
                                break;
                            case "Hawaii":
                                elasticity = -6;
                                break;
                            case "Idaho":
                                elasticity = -6;
                                break;
                            case "Illinois":
                                elasticity = -6;
                                break;
                            case "Indiana":
                                elasticity = -6;
                                break;
                            case "Iowa":
                                elasticity = -6;
                                break;
                            case "Kansas":
                                elasticity = -6;
                                break;
                            case "Kentucky":
                                elasticity = -6;
                                break;
                            case "Louisiana":
                                elasticity = -6;
                                break;
                            case "Maine":
                                elasticity = -6;
                                break;
                            case "Maryland":
                                elasticity = -6;
                                break;
                            case "Massachusetts":
                                elasticity = -3;
                                break;
                            case "Michigan":
                                elasticity = -6;
                                break;
                            case "Minnesota":
                                elasticity = -6;
                                break;
                            case "Mississippi":
                                elasticity = -6;
                                break;
                            case "Missouri":
                                elasticity = -6;
                                break;
                            case "Montana":
                                elasticity = -6;
                                break;
                            case "Nebraska":
                                elasticity = -6;
                                break;
                            case "Nevada":
                                elasticity = -2;
                                break;
                            case "New Hampshire":
                                elasticity = -6;
                                break;
                            case "New Jersey":
                                elasticity = -3;
                                break;
                            case "New Mexico":
                                elasticity = -6;
                                break;
                            case "New York":
                                elasticity = -3;
                                break;
                            case "North Carolina":
                                elasticity = -6;
                                break;
                            case "North Dakota":
                                elasticity = -6;
                                break;
                            case "Ohio":
                                elasticity = -6;
                                break;
                            case "Oklahoma":
                                elasticity = -6;
                                break;
                            case "Oregon":
                                elasticity = -6;
                                break;
                            case "Pennsylvania":
                                elasticity = -3;
                                break;
                            case "Rhode Island":
                                elasticity = -6;
                                break;
                            case "South Carolina":
                                elasticity = -6;
                                break;
                            case "South Dakota":
                                elasticity = -6;
                                break;
                            case "Tennessee":
                                elasticity = -6;
                                break;
                            case "Texas":
                                elasticity = -6;
                                break;
                            case "Utah":
                                elasticity = -6;
                                break;
                            case "Vermont":
                                elasticity = -6;
                                break;
                            case "Virginia":
                                elasticity = -6;
                                break;
                            case "Washington":
                                elasticity = -6;
                                break;
                            case "Washington DC":
                                elasticity = -6;
                                break;
                            case "West Virginia":
                                elasticity = -6;
                                break;
                            case "Wisconsin":
                                elasticity = -6;
                                break;
                            case "Wyoming":
                                elasticity = -6;
                                break;



                        }
                    }
                }
                Console.ReadKey();
            }*/

                Random random = new Random();
            double mathResult = Math.Round((random.NextDouble() * (0.005) + 0.03));

            // maybe can replace mathResult with two drawing coefficients used respectively on each individual fan-base
            double demand = mathResult * (getBase(A) * (1 - PWin(fo)) + getBase(B) * (1 - PWin(fo)));

            double price = 0; // average ticket price, up to user

            double attendance = -elasticity * price + demand;
            if (attendance > venue.Capacity) {
                attendance = venue.Capacity;
            }

            double revenue = attendance * price;
            Console.WriteLine(attendance + " tickets were sold of " + venue.Capacity + " available at an average price of " + price + " and for revenue of " + revenue + ".");

            // need to add hometown, nationality effects from both fighters + drawing power
            return attendance;
        }

        private static double DrawingPower(Fight Outcome, Fighter F)
        {
            double DrawingPower = 0;

            double DrawingCoefficient = 0.05;

            //DrawingPower = F.fans * DrawingCoefficient;

            // fighter drawing power should help determine the % of fans of a fighter that GO to fights. could be based on excitement (workrate/KO's) in ring + randomness. 

            return DrawingPower;
        }

        


        private static double PWin(FightOutcome fo)
        {
            return 1 / (1 + Math.Pow(10, (fo.Fighter2().Performance["Elo"] - fo.Fighter1().Performance["Elo"]) / 400));

        }


        



    }


}