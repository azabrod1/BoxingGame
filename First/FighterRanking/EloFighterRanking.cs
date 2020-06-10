using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;

namespace Boxing.FighterRating
{
    [Serializable()]
    //Thread safe (I hope) Elo Pool
    public class EloFighterRating : IFighterRating
    {
        [JsonProperty] private readonly ConcurrentDictionary<string, double> Ratings;

        [JsonProperty] public int K { get; }

        [JsonProperty] double InitElo { get; }

        public EloFighterRating(int k = 50, double initElo = 1500)
        {
            Ratings = new ConcurrentDictionary<string, double>();
            K = k;
            InitElo = initElo;
        }

        /**
	     * Calculate the expected score for the player with rating1
	     * 
	     * @param rating1	The player's rating
	     * @param rating2	The other player's rating
	     * @return			The expected score (0-1), where loss=0, draw=0.5, win=1
	     */
        public double CalculateExpectedScore(double Rating1, double Rating2)
        {
            return 1 / (1.0 + Math.Pow(10.0, (Rating2 - Rating1) / 400.0));
        }

        /**
         * Calculate the rating change for the player with rating1 and the given score
         * 
         * @param rating1	The player's rating
         * @param rating2	The other player's rating
         * @param score		The player's obtained score, where loss=0, draw=0.5, win=1
         * @return			The change in rating
         */
        public double CalculateRatingChange(double f1Rating, double f2Rating, double score)
        {
            return Math.Round(K * (score - CalculateExpectedScore(f1Rating, f2Rating)));
        }

        public bool AddFighter(Fighter fighter)
        {
            return Ratings.TryAdd(fighter.Name, InitElo);
        }

        public double Rating(Fighter fighter)
        {
            return Ratings[fighter.Name];
        }

        public double CalculateRatingChange(Fighter f1, Fighter f2, double score)
        {
            double delta = CalculateRatingChange(Rating(f1), Rating(f2), score);
            Ratings[f1.Name] += delta;
            Ratings[f2.Name] -= delta;

            f1.Performance["Elo"] = Ratings[f1.Name];
            f2.Performance["Elo"] = Ratings[f2.Name];


            return delta;
        }
    }
}
