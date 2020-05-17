using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static FightSim.Block;

namespace FightSim
{
    /* The purpose of the class is to keep track of basic fight stats for each minute, round and the whole fight
     * The class is additive so one can, for instance, add the rounds together to get a resulting summary object for the whole fight
     *
     * The hope is that this can serve as a stat tracker for varing granularities
     * 
     *
     */
    public class FightStats
    {
        public (double Fighter1, double Fighter2) Damage;
        public (int Fighter1, int Fighter2) Thrown;
        public (int Fighter1, int Fighter2) Landed;
        public (int Fighter1, int Fighter2) Jabs;
        public (int Fighter1, int Fighter2) KnockedDown;


        public FightStats(double F1Damage, double F2Damage, int F1Thrown, int F2Thrown, int F1Landed, int F2Landed,
            int F1Jabs, int F2Jabs, int F1KDs = 0, int F2KDs = 0)
        {
            this.Damage      = (F1Damage, F2Damage);
            this.Thrown      = (F1Thrown, F2Thrown);
            this.Landed      = (F1Landed, F2Landed);
            this.Jabs        = (F1Jabs,   F2Jabs);
            this.KnockedDown = (F1KDs,    F2KDs);
        }

        public override string ToString()
        {
            return this.FancyString("Fighter 1", "Fighter 2");
        }

        public string FancyString(string f1Name, string f2Name)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}\n", f1Name)
              .AppendFormat("Damage {0}\n", Damage.Fighter1)
              .AppendFormat("Thrown {0}\n", Thrown.Fighter1)
              .AppendFormat("Landed {0}\n", Landed.Fighter1)
              .AppendFormat("Jabs   {0}\n", Jabs.Fighter1)
              .AppendFormat("KDs    {0}\n", KnockedDown.Fighter1);


            sb.AppendFormat("{0}\n", f2Name)
              .AppendFormat("Damage {0}\n", Damage.Fighter2)
              .AppendFormat("Thrown {0}\n", Thrown.Fighter2)
              .AppendFormat("Landed {0}\n", Landed.Fighter2)
              .AppendFormat("Jabs   {0}\n", Jabs.Fighter2)
              .AppendFormat("KDs    {0}\n", KnockedDown.Fighter2);

            return sb.ToString();

        }

        public FightStats() {;}

        public FightStats(List<PunchResult> punches, FighterState F1, int F1KDs, int F2KDs)
        {
            this.Damage = (0d, 0d);
            this.Thrown = (0, 0);
            this.Landed = (0, 0);
            this.KnockedDown = (F1KDs, F2KDs);

            foreach (PunchResult punch in punches){
                if(punch.ThrownBy == F1)
                {
                    Thrown.Fighter1++;
                    Damage.Fighter1 += punch.Damage;
                    if (punch.Accuracy > 0)
                        ++Landed.Fighter1;
                    if (punch.Punch == PunchType.JAB)
                        ++Jabs.Fighter1;

                }
                else
                {
                    Thrown.Fighter2++;
                    Damage.Fighter2 += punch.Damage;
                    if (punch.Accuracy > 0)
                        ++Landed.Fighter2;
                    if (punch.Punch == PunchType.JAB)
                        ++Jabs.Fighter2;
                }
            }

        }
        
        public static FightStats operator +(FightStats A, FightStats B)
        {
            FightStats summary = new FightStats();
            summary.Append(A);
            summary.Append(B);
            return summary;
        }

        public void Append(FightStats that)
        { 
            this.Damage = (this.Damage.Fighter1 + that.Damage.Fighter1, this.Damage.Fighter2 + that.Damage.Fighter2);
            this.Thrown = (this.Thrown.Fighter1 + that.Thrown.Fighter1, this.Thrown.Fighter2 + that.Thrown.Fighter2);
            this.Landed = (this.Landed.Fighter1 + that.Landed.Fighter1, this.Landed.Fighter2 + that.Landed.Fighter2);
            this.Jabs   = (this.Jabs.Fighter1   + that.Jabs.Fighter1,   this.Jabs.Fighter2   + that.Jabs.Fighter2);
            this.KnockedDown = (this.KnockedDown.Fighter1 + that.KnockedDown.Fighter1, this.KnockedDown.Fighter2 + that.KnockedDown.Fighter2);
     
        }

                public double JabPercent(bool Fighter1)
        {
            if (Fighter1)
                return (double) 100 * Jabs.Fighter1 / Thrown.Fighter1;

            return (double) 100 * Jabs.Fighter2 / Thrown.Fighter2;
        }

    }



}
