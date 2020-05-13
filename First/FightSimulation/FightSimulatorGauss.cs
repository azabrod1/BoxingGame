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

        private BoxScore SimFight(FightState fight)
        {
            double result = -1; //Second fight ends

            for (int round = 1; round <= fight.Fight.RoundsScheduled; ++round)
            {
                for (int min = 0; min < 3 && result == -1; ++min)
                {
                    Block block = new Block(fight);

                    result = block.Play();

                    /*
                    if (opponent.IncrementHealth(-damage[result]) <= 0)
                    {
                        round = 12;
                        break;
                    }

                    opponent.IncrementHealth(opponent.RecoveryRate());
                    ++result;
                    */

                }

                //   Console.WriteLine("Damage " + (int)rndDam);

                //opponent.IncrementHealth(opponent.RecoveryRate());
                
            }

            return null;

        }

    }
    }
