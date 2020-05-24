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
            int[,] scorecards = new int[3,2];

            for (; fight.Round < fight.Fight.RoundsScheduled; fight.Round++)
            {
                fight.FightStats.Add(new FightStats()); //Add new stat page for the round
                var punchesThisRound = new List<Block.PunchResult>();

                for (int min = 0; min < Constants.BLOCKS_IN_ROUND; ++min)
                {
                    Block block = new Block(fight);
                    var blockOutcome = block.Play();

                    FightStats blockStats = new FightStats(blockOutcome.Punches, block.Knockdowns[0], block.Knockdowns[1]);
                    fight.FightStats[fight.Round].Append(blockStats);
                    punchesThisRound.AddRange(blockOutcome.Punches);

                    if (blockOutcome.Stoppage != -1)
                    {
                        timeOfStoppage = blockOutcome.Stoppage + min * 60 + fight.Round * 180;
                        goto FightStopped;
                    }

                    fight.F1.RecoverFor(60);
                    fight.F2.RecoverFor(60);
                }

                int[] roundScore;

                for (int j = 0; j < fight.Fight.Judges.Length; ++j)
                {
                    roundScore = fight.Fight.Judges[j].ScoreRound(punchesThisRound, fight.FightStats[fight.Round]);
                    scorecards[j,0] += roundScore[0];
                    scorecards[j,1] += roundScore[1];
                };

                fight.F1.RecoverFor(60);
                fight.F2.RecoverFor(60);
            }

        FightStopped:

            FightOutcome outcome = DeclareFightResult(fight, scorecards, timeOfStoppage);
            return outcome;
        }

        private FightOutcome DeclareFightResult(FightState fight, int[,] scorecards, int timeOfStoppage)
        {
            Fighter winner = null;
            FightOutcome outcome;
            MethodOfResult method = MethodOfResult.UD;

            if (timeOfStoppage != -1)
            {
                winner = fight.F1.Health <= 0 ? fight.Boxer2() : fight.Boxer1();
                outcome = new FightOutcome(timeOfStoppage, MethodOfResult.KO, winner, scorecards);
            }
            else
            {
                int[] cardsWon = { 0, 0 };

                for (int card = 0; card < scorecards.GetLength(0); ++card)
                {
                    if (scorecards[card, 0] > scorecards[card, 1])
                        ++cardsWon[0];
                    else if (scorecards[card, 0] < scorecards[card, 1])
                        ++cardsWon[1];
                }

                if (cardsWon[0] > cardsWon[1])
                    winner = fight.Boxer1();
                else if (cardsWon[0] < cardsWon[1])
                    winner = fight.Boxer2();

                if (Math.Abs(cardsWon[0] - cardsWon[1]) < 3)
                    method = MethodOfResult.MD;

                outcome = new FightOutcome(-1, method, winner, scorecards);
            }

            return outcome;
        }

    }
    }
