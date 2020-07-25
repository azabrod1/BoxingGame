using System;
namespace Main
{
    public class Financials
    {

        public Fight Fight { get; private set; }

        public Venue Venue { get; set; }
        public double VenueCost { get; private set; }
        public double TicketPrice { get; private set; }


        public double Interested { get; set; }
        public double Viewership { get; set; }
        public double Attendance { get; set; }
        

        public double FigherRedCost { get; set; }
        public double FigherBlueCost { get; set; }


        public Financials(Fight f)
        {
            this.Fight = f;
        }

        public void SetVenue (Venue venue, double venueCost, double ticketPrice)
        {
            this.Venue = venue;
            this.TicketPrice = ticketPrice;
            this.VenueCost = venueCost;
        }


        public double PL()
        {
            double pl = 0;

            // venue
            pl = Attendance * TicketPrice - VenueCost;

            // network pays - todo
            // pl =

            // PPV - todo
            // pl =

            // Paying fighters - todo
            // pl =

            return pl;




        }


    }
}
