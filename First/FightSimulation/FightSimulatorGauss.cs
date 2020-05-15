using System;
using Main;

namespace FightSim
{
    //Fight simulator with randomness simulated via normal distibution
    public class FightSimulatorGauss : FightSimulator
    {
        
        public FightSimulatorGauss()
        {
        }

        public FightOutcome SimulateFight(Fight fight)
        {
            FightState fightState = new FightState(fight);
            return null;


        }

        private FightOutcome SimFight(FightState fight)
        {
            double result = -1; //Second fight ends

            for (; fight.Round <= fight.Fight.RoundsScheduled; fight.Round++)
            {
                for (int min = 0; min < 3; ++min)
                {
                    Block block = new Block(fight);

                    var blockOutcome = block.Play();

                    //Incorperate into BoxScore somehow?
                    

                    if (blockOutcome.Stoppage != -1)
                        break;

                    fight.f1.RecoverFor(60);
                    fight.f2.RecoverFor(60);
                }

                fight.f1.RecoverFor(60);
                fight.f2.RecoverFor(60);

                //   Console.WriteLine("Damage " + (int)rndDam);

                //opponent.IncrementHealth(opponent.RecoveryRate());

            }

            return null;

        }

    }
    }
