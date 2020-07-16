using System;
using System.Collections.Generic;
using Main;
namespace FightSim
{
    //Tracks the state of the fight
    //Holds temp vars while Fight holds perm

    public class FightState
    {
        public int Minute { get; set; } = -1;
        public int Round { get { return Minute / 3; } set { Minute = value * 3; }  }

        public Fight Fight;
        public FighterState F1, F2;
        public double FightControl;
        public double FightDistance;        //Distance the fight is faught at
        public List<FightStats> FightStats; //Round By Round fight stats
        public int TimeOfStoppage;          //The second the fight ends
        public int[,] Scorecards;
        public bool Concluded { get { return TimeOfStoppage != -1 || 1+Minute == Fight.RoundsScheduled * 3; } }
        public List<List<Block.PunchResult>> Punches { get; set; }
        public FightOutcome Outcome { get; internal set; }

        public const double REACH_ADV = 0.025;

        public FightState(Fight fight)
        {
            this.Fight = fight;
            this.FightControl = CalcFightControl();

            this.F1 = new FighterState(fight.Fighter1());
            this.F2 = new FighterState(fight.Fighter2());

            //We should add some randomness here?
            FightDistance = FightControl * F1.PreferredDistance(F2) + (1-FightControl) * F2.PreferredDistance(F1);
            FightStats = new List<FightStats>();

            this.TimeOfStoppage = -1;
            this.Scorecards     = new int[3, 2];
            this.Punches = new List<List<Block.PunchResult>>();

            InitNextRound();
        }

        public void NextRound()
        {
            do
                NextMinute();
            while (!Concluded && (1 + Minute) % 3 != 0);
        }

        public void InitNextRound()
        {
            FightStats.Add(new FightStats()); //Add new stat page for the round
            Punches.Add(new List<Block.PunchResult>());
        }

        public void EndOfRound()
        {
            int round = CurrentRound();

            for (int j = 0; j < Fight.Judges.Length; ++j) //Score Round
            {
                int[] roundScore = Fight.Judges[j].ScoreRound(Punches[round], FightStats[round]);
                Scorecards[j, 0] += roundScore[0];
                Scorecards[j, 1] += roundScore[1];
            };

            if (Concluded) //If not concluded, set up the next round
                Outcome = DetermineFightResult();
            else
            {
                F1.RecoverFor(60);
                F2.RecoverFor(60);
                InitNextRound();
            }
        }
        
        public void NextMinute()
        {
            if (Concluded)
                throw new Exception("Fight already finished");

            int round = ++Minute / 3;
            Block block = new Block(this);
            var blockOutcome = block.Run();

            FightStats blockStats = new FightStats(blockOutcome.Punches, block.Knockdowns[0], block.Knockdowns[1]);
            FightStats[round].Append(blockStats);
            Punches[round].AddRange(blockOutcome.Punches);

            if (blockOutcome.Stoppage != -1)
            {
                TimeOfStoppage = blockOutcome.Stoppage + Minute * 60;
                EndOfRound();
                return;
            }

            F1.RecoverFor(60);
            F2.RecoverFor(60);

            if ((1 + Minute) % 3 == 0)
                EndOfRound();
        }

        public FightOutcome SimulateFight()
        {
            while (!Concluded)
                NextMinute();

            return Outcome;
        }

        //Percent of match control that Fighter1() gets
        public double CalcFightControl()
        {
            double control = Utility.AttributeRatioCustom(Fight.Fighter1().RingGen, Fight.Fighter2().RingGen, 2d, Fight.Fighter1().FootWork, Fight.Fighter2().FootWork, Constants.SQR_ROOT_TWO);
            return control;
        }

        public Fighter Boxer1()
        {
            return Fight.Fighter1();
        }

        public Fighter Boxer2()
        {
            return Fight.Fighter2();
        }

        //Percent of punches a fighter gains or loses due to reach advantage/disadvantage 
        public double ReachBuff()
        {
            double reachDifference = Boxer1().Reach - Boxer2().Reach;
            double ringGen = reachDifference > 0? Boxer1().RingGen : Boxer2().RingGen;
            return reachDifference * REACH_ADV * FightDistance*ringGen*0.01;
        }

        public int CurrentRound()
        {
            return Minute / 3;
        }

        private FightOutcome DetermineFightResult()
        {
            Fighter winner = null;
            FightOutcome outcome;
            MethodOfResult method = MethodOfResult.UD;

            if (TimeOfStoppage != -1)
            {
                winner = F1.Health <= 0 ? Boxer2() : Boxer1();
                outcome = new FightOutcome(TimeOfStoppage, MethodOfResult.KO, winner, Scorecards, Fight);
            }
            else
            {
                int[] cardsWon = { 0, 0 };

                for (int card = 0; card < Scorecards.GetLength(0); ++card)
                {
                    if (Scorecards[card, 0] > Scorecards[card, 1])
                        ++cardsWon[0];
                    else if (Scorecards[card, 0] < Scorecards[card, 1])
                        ++cardsWon[1];
                }

                if (cardsWon[0] > cardsWon[1])
                    winner = Boxer1();
                else if (cardsWon[0] < cardsWon[1])
                    winner = Boxer2();

                if (Math.Abs(cardsWon[0] - cardsWon[1]) < 3)
                    method = MethodOfResult.MD;

                outcome = new FightOutcome(-1, method, winner, Scorecards, Fight);
            }

            return outcome;
        }
    }
}
