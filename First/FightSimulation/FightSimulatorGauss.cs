using System;
using System.Collections.Generic;
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
            return SimFight(fightState);
        }

        public (FightOutcome outcome, List<FightStats> Stats) SimulateFightWithDetails(Fight fight)
        {
            FightState fightState = new FightState(fight);
            FightOutcome outcome = SimFight(fightState);
            return (outcome, fightState.FightStats);
        }

        private FightOutcome SimFight(FightState fight)
        {
            int timeOfStoppage = -1; //The second the fight ends

            for (; fight.Round < fight.Fight.RoundsScheduled; fight.Round++)
            {
                fight.FightStats.Add(new FightStats()); //Add new stat page for the round

                for (int min = 0; min < Constants.BLOCKS_IN_ROUND; ++min)
                {
                    Block block = new Block(fight);

                    var blockOutcome = block.Play();
                    FightStats blockStats = new FightStats(blockOutcome.Punches, fight.F1, 0, 0);
                    fight.FightStats[fight.Round].Append(blockStats);

                    if (blockOutcome.Stoppage != -1)
                    {
                        timeOfStoppage = blockOutcome.Stoppage + min * 60 + fight.Round * 180;
                        goto FightStopped;
                    }

                    fight.F1.RecoverFor(60);
                    fight.F2.RecoverFor(60);

                }

                fight.F1.RecoverFor(60);
                fight.F2.RecoverFor(60);
            }

            FightStopped:

            FightOutcome outcome = DeclareFightResult(fight, timeOfStoppage);
            return outcome;
        }

        private FightOutcome DeclareFightResult(FightState fight, int timeOfStoppage)
        {
            Fighter winner;
            FightOutcome outcome;

            if (timeOfStoppage != -1)
            {
                winner = fight.F1.Health <= 0 ? fight.Boxer2() : fight.Boxer1();
                outcome = new FightOutcome(timeOfStoppage, MethodOfResult.KO, winner);
            }
            else
            {
                winner = (fight.FightStats.AverageLanded(true)> fight.FightStats.AverageLanded(false))? fight.Boxer1() : fight.Boxer2();
                outcome = new FightOutcome(timeOfStoppage, MethodOfResult.UD, winner);
            }

            return outcome;
        }

    }
    }
