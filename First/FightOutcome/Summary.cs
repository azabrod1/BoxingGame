using System;
namespace Boxing.FightOutcome
{
    /* The purpose of the class is to keep track of basic fight stats for each minute, round and the whole fight
     * The class is additive so one can, for instance, add the rounds together to get a resulting summary object for the whole fight
     *
     * The hope is that this can serve as a stat tracker for varing granularities
     * 
     *
     */
    public class Summary
    {
        public (double Fighter1, double Fighter2) Damage;
        public (double Fighter1, double Fighter2) Thrown;
        public (double Fighter1, double Fighter2) Landed;
        public (double Fighter1, double Fighter2) KnockedDown;


        public Summary(double F1Damage, double F2Damage, double F1Thrown, double F2Thrown, double F1Landed, double F2Landed, double F1KDs = 0, double F2KDs = 0)
        {
            this.Damage = (F1Damage, F2Damage);
            this.Thrown = (F1Thrown, F2Thrown);
            this.Landed = (F1Landed, F2Landed);
            this.KnockedDown = (F1KDs, F2KDs);

        }

        public Summary() {;}

        
        public static Summary operator +(Summary A, Summary B)
        {
            Summary summary = new Summary
            {
                Damage = (A.Damage.Fighter1 + B.Damage.Fighter1, A.Damage.Fighter2 + B.Damage.Fighter2),
                Thrown = (A.Thrown.Fighter1 + B.Thrown.Fighter1, A.Thrown.Fighter2 + B.Thrown.Fighter2),
                Landed = (A.Landed.Fighter1 + B.Landed.Fighter1, A.Landed.Fighter2 + B.Landed.Fighter2),
                KnockedDown = (A.KnockedDown.Fighter1 + B.KnockedDown.Fighter1, A.KnockedDown.Fighter2 + B.KnockedDown.Fighter2),
            };

            return summary;

        }



    }



}
