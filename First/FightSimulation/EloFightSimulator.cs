using System;
using System.Linq;
using Main;
namespace FightSim
{
    public class EloFightSimulator : FightSimulator
    {

        //public static double ELO_MAX_INIT = 1000.0;

        public EloFightSimulator()
        {
        }

        public FightOutcome SimulateFight(Main.Fight fight)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            Fighter winner;

            int f1 = int.Parse(String.Join("", fight.Fighter1().Name.Where(char.IsDigit)));
            int f2 = int.Parse(String.Join("", fight.Fighter2().Name.Where(char.IsDigit)));
           
            if (f1 > f2)
            {
                // fighter 1 won
                updateElo(fight.Fighter1(), fight.Fighter2());
                winner = fight.Fighter1();

                fight.Fighter1().Record.Wins++;
                fight.Fighter2().Record.Losses++;

            }
            else
            {
                updateElo(fight.Fighter2(), fight.Fighter1());
                winner = fight.Fighter2();
                fight.Fighter2().Record.Wins++;
                fight.Fighter1().Record.Losses++;
            }
            FightOutcome fo = new FightOutcome(0, FightSim.MethodOfResult.NC, winner, null, fight.fighters);
            //fo.Viewership = getNetworkViewers(fight.Fighter1().Record.Rank, fight.Fighter2().Record.Rank);
            fight.Outcome = fo;
            return fo;
        }


        private static void updateElo(Fighter winner, Fighter loser)
        {
            //double delta = 0.45 * loser.Record.Rank - 0.05 * winner.Record.Rank;

            double delta =  eloDelta(winner.Record.Rank, loser.Record.Rank);
            winner.Record.Rank += delta;
            loser.Record.Rank -= delta;

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

        //private static double getNetworkViewers(double elo1, double elo2)
        //{
            //double risk1 = 1 / Math.Abs(elo1 - elo2);
            //double risk2 = 1 - risk1;

            //return interested + (risk1)fighter1.base + (risk2)fighter2.base;
            //return MathUtils.Gauss(((elo1 + elo2) / 2), 100);
        }

    }
//}
