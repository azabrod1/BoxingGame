using System.Collections.Generic;
using Main;

namespace Boxing.FighterRating
{

    public interface IPlayerRating
    {
        /**Get Elo for a player! 
         * IMPLEMENT ME! 
         */
        public double Rating(Fighter fighter);

        /**Add new fighter to the Elo Rating
         * Return false if add fails becuase fighter already exists
         * IMPLEMENT ME! 
         */
        public bool AddFighter(Fighter fighter);

        public void AddFighters(IEnumerable<Fighter> fighters)
        {
            foreach (var fighter in fighters)
                AddFighter(fighter);
        }

        /**
         * Calculate the rating change for the player with Rating1 and the given score
         * 
         * @param f1Rating	The player's rating
         * @param f2Rating	The other player's rating
         * @param score     The player's obtained score, where i.e. koLoss 0, loss=0.1, draw=0.5, win=0.9 koWin = 1
         * @return			The change in rating
         */


        //public double CalculateRatingChange(Fighter f1, Fighter f2, FightSim.FightOutcome outcome)
        //{
        //    if (double.IsNaN(Elo(f1)))
        //        AddFighter(f1);

        //    if (double.IsNaN(Elo(f2)))
        //        AddFighter(f2);

        //    return CalculateRatingChange(Elo(f1), Elo(f2), outcome);

        //}

        ///THE BELOW METHODS ARE PUBLIC BUT I THINK THEY SHOULD BE USED MORE FOR TESTING
        ///AND TWEAKING HOW THE ALGOS WORK INTERNALLY. THE GAME SHOULD ONLY NEED TO TOUCH THE ABOVE TWO
        ///

        /**
         * Calculate the rating change for the player with Rating1 and the given score
         * 
         * @param f1Rating	The player's rating
         * @param f2Rating	The other player's rating
         * @param score     The player's obtained score, where i.e. loss=0, draw=0.5, win=0.9 koWin = 1
         * @return			The change in rating
         * 
         * IMPLEMENT ME!
         */

        // public double CalculateRatingChange(double f1Rating, double f2Rating, double score);

        /**
         * Calculate the rating change for the player with Rating1 and the given score
         * 
         * @param f1	    The player
         * @param f2    	The other player 
         * @param score     The player's obtained score, where i.e. loss=0, draw=0.5, win=0.9 koWin = 1
         * @return			The change in rating
         * 
         *  IMPLEMENT ME!
         */

        public double CalculateRatingChange(Fighter f1, Fighter f2, double score);

        /**
         * Calculate the rating change for the player with Rating1 and the given score
         * 
         * @param f1Rating	The player's rating
         * @param f2Rating	The other player's rating
         * @param score     The player's obtained score, where i.e. koLoss 0, loss=0.1, draw=0.5, win=0.9 koWin = 1
         * @return			The change in rating
         */

        public double CalculateRatingChange(Fighter f1, Fighter f2, FightSim.FightOutcome outcome)
        {
            double score = 0.5;

            //No Contest means no change almost by definition
            if (outcome.Method == FightSim.MethodOfResult.NC)
                return 0;

            if (outcome.IsDraw())
                return score;

            score = (outcome.WinnerNum() == 0) ? 1 : 0;

            if (outcome.IsKO())
                if (score == 1)
                    score = 0.9;
                else
                    score = 0.1;

            return CalculateRatingChange(f1, f2, score);
        }

        public double CalculateRatingChange(FightSim.FightOutcome outcome)
        {
            double score = 0.5;

            //No Contest means no change almost by definition
            if (outcome.Method == FightSim.MethodOfResult.NC)
                return 0;

            if (outcome.IsDraw())
                return score;

            score = (outcome.WinnerNum() == 0) ? 1 : 0;

            if (outcome.IsKO())
                if (score == 1)
                    score = 0.9;
                else
                    score = 0.1;

            return CalculateRatingChange(outcome.Fighter1(), outcome.Fighter2(), score);
        }

    }

}
