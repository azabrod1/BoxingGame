using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;
using System.Collections.Generic;
using FightSim;
using System.Text;
using System.Xml;
 

namespace Main
{

    public enum FPType 
    {
        ACTIVE,
        FANS,
        CASUALS,
        FOLLOWERS,
        INTERESTED,
        ELO,
        DRAWING_POWER
    }

    public static class FighterPopularity
    {
        // simple dictionaries to hold country and weight popularity coefficients
        // #todo: integrate with the rest of the program and persist the coefficients in XLM

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


            if (!f.Performance.ContainsKey(FPType.ACTIVE))
            {
                // first time the method is called for a new fighter
                // need to populate initial values
                //(Active is an indicator the fighter is initialized)
                f.Performance[FPType.ACTIVE] = 1;

                // todo need to make proper values

                f.Performance[FPType.FANS] = 20;
                f.Performance[FPType.CASUALS] = 5;
                f.Performance[FPType.FOLLOWERS] = 5;
                f.Performance[FPType.ELO] = 500;
                f.Performance[FPType.DRAWING_POWER] = 100;

            }
            else
            {

                fans = f.Performance[FPType.FANS];
                casuals = f.Performance[FPType.CASUALS];
                followers = f.Performance[FPType.FOLLOWERS];

                // do the calculatiuon
                if (fans == 0)
                    fans = 1000;


                fans = fans + 1;
                // end of calculation


                f.Performance[FPType.FANS] = fans;
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
        public static void UpdatePopularity(Fight f)
        {

            Fighter A = f.FighterRed;
            Fighter B = f.FighterBlue;

            // in case popularity has never been set up
            // set the initial values
            // (should not happen)
            if (!A.Performance.ContainsKey(FPType.ACTIVE)){
                UpdatePopularity(A);
            }
            if (!B.Performance.ContainsKey(FPType.ACTIVE)){
                UpdatePopularity(B);
            }



            double fans1 = A.Performance[FPType.FANS];
            double fans2 = B.Performance[FPType.FANS];

            double casuals1 = A.Performance[FPType.CASUALS];
            double casuals2 = B.Performance[FPType.CASUALS];

            double followers1 = A.Performance[FPType.FOLLOWERS];
            double followers2 = B.Performance[FPType.FOLLOWERS];

            double drawingPower1 = A.Performance[FPType.DRAWING_POWER];
            double drawingPower2 = B.Performance[FPType.DRAWING_POWER];

            Console.WriteLine("Fighter1: " + ToString(A));
            Console.WriteLine("Fighter2: " + ToString(B));

            double CountryCoeff1 = A.Nationality.PopularityBuff;
            double CountryCoeff2 = B.Nationality.PopularityBuff;
            double WeightCoeff = ((WeightClass)A.Weight).Popularity;


            // do the calculation

            if (A == f.Outcome.Winner)
            {
                //winner
                double delta = 0.09 * casuals1 * (1 - PWin(f)) * WeightCoeff * CountryCoeff1;
                delta = MathUtils.Gauss(delta, 0.5);
                fans1 += delta;
                casuals1 -= delta;

                delta = 0.4 * f.Interested * (1 - PWin(f)) * WeightCoeff * CountryCoeff1;
                delta = MathUtils.Gauss(delta, 0.5);
                casuals1 += delta;
                //fo.Interested =- delta;

                delta = 0.09 * casuals1 * (1 - PWin(f)) * WeightCoeff * CountryCoeff1;
                delta = MathUtils.Gauss(delta, 0.5);
                followers1 += delta;
                casuals1 -= 0.18 * casuals1;

                //loser

                delta = 0.1 * fans2 * (1 - PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                fans2 -= delta;
                casuals2 += delta;

                delta = 0.1 * casuals2 * (1 - PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                casuals2 -= delta;
                //fo.Interested = +delta;

                delta = 0.1 * followers2 * (1 - PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                followers2 -= delta;
                casuals2 += delta;

            }
            else if (B == f.Outcome.Winner)
            {

                //loser

                double delta = 0.09 * casuals1 * (PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                fans1 -= delta;
                casuals1 += delta;

                delta = 0.4 * f.Interested * (PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                casuals1 -= delta;
                //fo.Interested =+ delta;

                delta = 0.09 * casuals1 * (PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                followers1 -= delta;
                casuals1 += 0.18 * casuals1;

                //winner

                delta = 0.1 * fans2 * (PWin(f)) * WeightCoeff * CountryCoeff2;
                delta = MathUtils.Gauss(delta, 0.5);
                fans2 += delta;
                casuals2 -= -delta;

                delta = 0.1 * casuals2 * (PWin(f)) * WeightCoeff * CountryCoeff2;
                delta = MathUtils.Gauss(delta, 0.5);
                casuals2 += delta;
                //fo.Interested =- delta;

                delta = 0.1 * followers2 * (PWin(f)) * WeightCoeff * CountryCoeff2;
                delta = MathUtils.Gauss(delta, 0.5);
                followers2 += delta;
                casuals2 -= delta;

            }


            // end of calculation

            A.Performance[FPType.FANS] = fans1;
            B.Performance[FPType.FANS] = fans2;
            A.Performance[FPType.CASUALS] = casuals1;
            B.Performance[FPType.CASUALS] = casuals2;
            A.Performance[FPType.FOLLOWERS] = followers1;
            B.Performance[FPType.FOLLOWERS] = followers2;

            
            f.Viewership = FightViewers(f);
            //f.Attendance = 

            //Console.WriteLine(ToString(f.Outcome));
            Console.WriteLine("Fighter1: " + ToString(A));
            Console.WriteLine("Fighter2: " + ToString(B) + "\n");



        }

        public static string ToString(Fighter f)
        {

            var p = f.Performance;
            string s = $"Fighter {f.Name}:Fans={p[FPType.FANS]:N0},Followers={p[FPType.FOLLOWERS]:N0}," +
                $"Casuals={p[FPType.CASUALS]:N0},Elo={p[FPType.ELO]:N0}";

            return s;
        }

        public static string ToString(Fight f)
        {
            return ($"Viewership:{f.Viewership:N0}");

        }


        // PRIVATE METHODS

        private static double FightViewers(Fight f)
        {
            double viewers;


            Fighter A = f.FighterRed;
            Fighter B = f.FighterBlue;

            WeightClass w1 = (WeightClass)A.Weight;

            
            f.Interested = MathUtils.Gauss(((A.Performance[FPType.ELO] + B.Performance[FPType.ELO]) / 2), 100) *
                (A.Nationality.PopularityBuff * B.Nationality.PopularityBuff * w1.Popularity + A.Belts + B.Belts);



            viewers = f.Interested + (PWin(f)) * A.Performance[FPType.FOLLOWERS] + (1 - PWin(f) * B.Performance[FPType.FOLLOWERS])
                + (PWin(f)) * A.Performance[FPType.FANS] + (1 - PWin(f) * B.Performance[FPType.FANS]);


            return viewers;



        }

        private static double getBase(Fighter F)
        {
            return F.Performance[FPType.FOLLOWERS] + F.Performance[FPType.FANS];
        }

        private static double fightAttendance(Fight fight, Venue venue, double ticketPrice)

        {

            Fighter A = fight.FighterRed;
            Fighter B = fight.FighterBlue;

            double elasticity = venue.Elasticity;

            Random random = new Random();
            double mathResult = Math.Round((random.NextDouble() * (0.005) + 0.03));

            // maybe can replace mathResult with two drawing coefficients used respectively on each individual fan-base

            double locationBuff1 = City.LocationBuff(A, venue);
            double locationBuff2 = City.LocationBuff(B, venue);

            double demand = mathResult * (getBase(A) * A.Performance[FPType.DRAWING_POWER] * (1 - PWin(fight)) * locationBuff1
                + getBase(B) * B.Performance[FPType.DRAWING_POWER] * (1 - PWin(fight)) * locationBuff2);

            // elasticity also gets better if your hometown is near where you're fighting or if you have a conducive nationality to where you're fighting
            // puerto ricans sell well in new york
            // mexicans sell well in vegas, texas, california, arizona
            // polish people sell well in new york, florida
            // ukrainians sell well in chicago
            // hispanics and americans and latinos and puerto ricans sell well everywhere really
            // cubans sell well in florida
            // pennsylvania
            // irish fighters

            double price = 0; // average ticket price, up to user

            

            double attendance = -elasticity * price + demand;
            if (attendance > venue.Capacity)
            {
                attendance = venue.Capacity;
            }

            double revenue = attendance * price;
            Console.WriteLine(attendance + " tickets were sold of " + venue.Capacity + " available at an average price of " + price + " and for revenue of " + revenue + ".");

            // need to add hometown, nationality effects from both fighters + drawing power
            return attendance;
        }

        private static void AdjustDrawingPower(Fight f)
        {
            Random random = new Random();
            double mathResult = Math.Round((random.NextDouble() * (0.005) + 0.03));
            if (f.Outcome.IsKO())
            {
                mathResult = mathResult * 3;
            }

            Fighter A = f.FighterRed;
            Fighter B = f.FighterBlue;

            if (A == f.Outcome.Winner)
            {
                A.Performance[FPType.DRAWING_POWER] += A.Performance[FPType.DRAWING_POWER] * mathResult * (1 - PWin(f));
                B.Performance[FPType.DRAWING_POWER] -= B.Performance[FPType.DRAWING_POWER] * mathResult * (PWin(f));
            } else if (B == f.Outcome.Winner)
            {
                A.Performance[FPType.DRAWING_POWER] -= A.Performance[FPType.DRAWING_POWER] * mathResult * (1 - PWin(f));
                B.Performance[FPType.DRAWING_POWER] += B.Performance[FPType.DRAWING_POWER] * mathResult * (PWin(f));
            }

            
            //DrawingPower = F.fans * DrawingCoefficient;

            // fighter drawing power should help determine the % of fans of a fighter that GO to fights. could be based on excitement (workrate/KO's) in ring + randomness. 

        }



        private static double PWin(Fight f)
        {
            return 1 / (1 + Math.Pow(10, (f.FighterBlue.Performance[FPType.ELO] - f.FighterRed.Performance[FPType.ELO]) / 400));
        }


    }

}
