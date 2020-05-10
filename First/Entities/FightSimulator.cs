using System;
using Fighting;
namespace Main
{
    public class FightSimulator : FightSim
    {
        public FightSimulator()
        {

        }

        public Fight simulate(Fight f)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            if (rand.Next(0,2) > 0)
            {
                // fighter 1 won
                updateElo(f.b1, f.b2);
                f.result = Result.FIGHTER1_WON;
            }
            else
            {
                updateElo(f.b2, f.b1);
                f.result = Result.FIGHTER2_WON;

            }
            // does not handle draw
            return f;
        }

        private void updateElo(Fighter winner, Fighter loser)
        {
            double delta = eloDelta(winner.elo, loser.elo);
            winner.previous_elo = winner.elo;
            loser.previous_elo = loser.elo;
            winner.elo += delta;
            loser.elo -= delta;
        }


        private double eloDelta(double eloW, double eloL)
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
