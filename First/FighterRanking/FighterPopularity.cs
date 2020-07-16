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

            Fighter W = f.Winner;
            Fighter L = f.Loser;

            if (W == null)
            {
                // todo handle draw here

                return;
            }

            // in case popularity has never been set up
            // set the initial values
            // (should not happen)
            if (!W.Performance.ContainsKey(FPType.ACTIVE)){
                UpdatePopularity(W);
            }
            if (!L.Performance.ContainsKey(FPType.ACTIVE)){
                UpdatePopularity(L);
            }

            

            double fansW = W.Performance[FPType.FANS];
            double fansL = L.Performance[FPType.FANS];

            double casualsW = W.Performance[FPType.CASUALS];
            double casualsL = L.Performance[FPType.CASUALS];

            double followersW = W.Performance[FPType.FOLLOWERS];
            double followersL = L.Performance[FPType.FOLLOWERS];

            double drawingPowerW = W.Performance[FPType.DRAWING_POWER];
            double drawingPowerL = L.Performance[FPType.DRAWING_POWER];

            //Console.WriteLine("Fighter1: " + ToString(A));
            //Console.WriteLine("Fighter2: " + ToString(B));

            double CountryCoeffW = W.Nationality.PopularityBuff;
            double CountryCoeffL = L.Nationality.PopularityBuff;
            double WeightCoeff = ((WeightClass)W.Weight).Popularity;


            // do the calculation

            
                //winner
                double delta = 0.09 * casualsW * (1 - PWin(f)) * WeightCoeff * CountryCoeffW;
                delta = MathUtils.Gauss(delta, 0.5);
                fansW += delta;
                casualsW -= delta;

                delta = 0.4 * f.Interested * (1 - PWin(f)) * WeightCoeff * CountryCoeffW;
                delta = MathUtils.Gauss(delta, 0.5);
                casualsW += delta;
                //fo.Interested =- delta;

                delta = 0.09 * casualsW * (1 - PWin(f)) * WeightCoeff * CountryCoeffW;
                delta = MathUtils.Gauss(delta, 0.5);
                followersW += delta;
                casualsW -= 0.18 * casualsW;

                //loser

                delta = 0.1 * fansL * (1 - PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                fansL -= delta;
                casualsL += delta;

                delta = 0.1 * casualsL * (1 - PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                casualsL -= delta;
                //fo.Interested = +delta;

                delta = 0.1 * followersL * (1 - PWin(f));
                delta = MathUtils.Gauss(delta, 0.5);
                followersL -= delta;
                casualsL += delta;

            

            // end of calculation

            W.Performance[FPType.FANS] = fansW;
            L.Performance[FPType.FANS] = fansL;
            W.Performance[FPType.CASUALS] = casualsW;
            L.Performance[FPType.CASUALS] = casualsL;
            W.Performance[FPType.FOLLOWERS] = followersW;
            L.Performance[FPType.FOLLOWERS] = followersL;

            
            f.Viewership = FightViewers(f);
            f.Attendance = fightAttendance(f, f.Venue, f.TicketPrice);

            //Console.WriteLine(ToString(f.Outcome));
            Console.WriteLine("Fighter1: " + ToString(W));
            Console.WriteLine("Fighter2: " + ToString(L) + "\n");

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
