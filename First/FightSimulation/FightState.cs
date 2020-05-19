using System;
using System.Collections.Generic;
using Main;
namespace FightSim
{
    //Tracks the state of the fight
    //Holds temp vars while Fight holds perm

    public class FightState
    {
        public int Round = 0;

        public Fight Fight;
        public FighterState F1, F2;
        public double FightControl;
        public double FightDistance;     //Distance the fight is faught at
        public List<FightStats> FightStats; //Round By Round fight stats 

        public const double REACH_ADV = 0.025;

        public Fighter Boxer1()
        {
            return Fight.b1;
        }

        public Fighter Boxer2()
        {
            return Fight.b2;
        }

        public FightState(Fight fight)
        {
            this.Fight = fight;
            this.FightControl = CalcFightControl();

            this.F1 = new FighterState(fight.b1);
            this.F2 = new FighterState(fight.b2);

            //We should add some randomness here?
            FightDistance = FightControl * F1.PreferredDistance(F2) + (1-FightControl) * F2.PreferredDistance(F1);
            FightStats = new List<FightStats>();
        }

        //Percent of match control that b1 gets
        public double CalcFightControl()
        {
            double control = Utility.AttributeRatioCustom(Fight.b1.RingGen, Fight.b2.RingGen, 2d, Fight.b1.FootWork, Fight.b2.FootWork, Constants.SQR_ROOT_TWO);
            return control;
        }

        public void RegisterKnockdown(FighterState Puncher)
        {
            if (Puncher == F1)
                FightStats[Round].Knockdowns.Fighter1++;
            else
                FightStats[Round].Knockdowns.Fighter2++;

        }

        //Percent of punches a fighter gains or loses due to reach advantage/disadvantage 
        public double ReachBuff()
        {
            double reachDifference = Boxer1().Reach - Boxer2().Reach;
            double ringGen = reachDifference > 0? Boxer1().RingGen : Boxer2().RingGen;
            return reachDifference * REACH_ADV * FightDistance*ringGen*0.01;
        }


    }
}
