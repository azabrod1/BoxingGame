using System;
using Main;
namespace Fighting
{
    public class EloFightSimulator : FightSimulator
    {

        public static double ELO_MAX_INIT = 1000.0;

        public EloFightSimulator()
        {
        }



        public FightOutcome SimulateFight(Main.Fight fight)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            Fighter winner;
            if (rand.Next(0, 2) > 0)
            {
                // fighter 1 won
                updateElo(fight.b1, fight.b2);
                winner = fight.b1;
                fight.b1.record.Wins++;
                fight.b2.record.Losses++;

            }
            else
            {
                updateElo(fight.b2, fight.b1);
                winner = fight.b2;
                fight.b2.record.Wins++;
                fight.b1.record.Losses++;
            }
            return new FightOutcome(0, Fighting.MethodOfResult.NC, winner);
        }

        

        private static void updateElo(Fighter winner, Fighter loser)
        {
            double delta = eloDelta(winner.Rank, loser.Rank);
            //winner.previous_elo = winner.rank;
            //loser.previous_elo = loser.rank;
            winner.Rank += delta;
            loser.Rank -= delta;
        }


        private static double eloDelta(double eloW, double eloL)
        {
            //CalculateELO(ref m_rating, ref loser.m_rating, 1);
            double eloK = 32;

            //P2 = (1.0 / (1.0 + pow(10, ((rating2 – rating1) / 400))));
            double expectationToWin = 1.0 / (1.0 + Math.Pow(10.0, (eloL - eloW) / 400.0));
            //delta  = K*(Actual Score – Expected score);
            double delta = eloK * (1.0 - expectationToWin);
            //Console.WriteLine(">>>winner rating = {0} loser rating = {1} P(exp) = {2} delta = {3}", m_rating, loser.m_rating, expectationToWin, delta);
            return delta;
        }


    }
}
