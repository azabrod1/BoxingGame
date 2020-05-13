using System;
using Main;
namespace FightSim
{
    //Tracks the state of the fight
    //Holds temp vars while Fight holds perm

    public class FightState
    {
        public int Round = 1;

        public Fight Fight;
        public FighterState f1, f2;
        public double fightControl;
        public double FightDistance; //Distance the fight is faught at
        public BoxScore boxScore;

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
            this.fightControl = FightControl();

            this.f1 = new FighterState(fight.b1);
            this.f2 = new FighterState(fight.b2);

            //We should add some randomness here?
            FightDistance = fightControl * f1.PreferredDistance(f2) + (1-fightControl) * f2.PreferredDistance(f1);

            this.boxScore = new BoxScore();
        }

        //Percent of match control that b1 gets
        public double FightControl()
        {
            double control = Utility.AttributeRatioCustom(Fight.b1.RingGen, Fight.b2.RingGen, 2d, Fight.b1.FootWork, Fight.b2.FootWork, Constants.SQR_ROOT_TWO);
            return control;
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
