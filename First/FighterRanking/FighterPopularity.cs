using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;


namespace Boxing.FighterRanking
{
    public class FighterPopularity
    {

        private struct PopularityStruct
        {
            public double Fans;
            public double Followers;
            public double Coefficient;

            public PopularityStruct(double Fans, double Followers, double Coefficient)
            {
                this.Fans = Fans;
                this.Followers = Followers;
                this.Coefficient = Coefficient;
            }

            public double Base {
                get => Fans + Followers;
                }


        }



        [JsonProperty] private readonly ConcurrentDictionary<string, double> Popularity;

        public bool AddFighter(Fighter fighter)
        {
            PopularityStruct p = new PopularityStruct(100, 100, 1);
            return Popularity.TryAdd(fighter.Name, p);
        }


    }
}
