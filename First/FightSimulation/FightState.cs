using System;
using Main;
namespace Fighting
{
    //Tracks the state of the fight
    public class FightState
    {
        //Holds temp vars while Fight holds perm
        public int round = 1;

        public Fight fight;
        public FighterState f1, f2;
        public double fightControl;
        public double fightDistance; //Distance fight is faught at

        public const double REACH_ADV = 0.025;

        public Fighter Boxer1()
        {
            return fight.b1;
        }

        public Fighter Boxer2()
        {
            return fight.b2;
        }

        public FightState(Fight fight)
        {
            this.fight = fight;
            this.fightControl = FightControl();

            this.f1 = new FighterState(fight.b1);
            this.f2 = new FighterState(fight.b2);

            fightDistance = fightControl * f1.PreferredDistance(f2) + (1-fightControl) * f2.PreferredDistance(f1);
        }

        //Percent of match control that b1 gets
        public double FightControl()
        {
            double control = Utility.AttributeRatioCustom(fight.b1.RingGen, fight.b2.RingGen, 2d, fight.b1.FootWork, fight.b2.FootWork, Constants.SqrRootTwo);
            return control;
        }

        //Percent of punches a fighter gains or loses due to reach advantage/disadvantage 
        public double ReachBuff()
        {
            double reachDifference = Boxer1().Reach - Boxer2().Reach;
            double ringGen = reachDifference > 0? Boxer1().RingGen : Boxer2().RingGen;
            return reachDifference * REACH_ADV * fightDistance*ringGen*0.01;
        }


    }
}
