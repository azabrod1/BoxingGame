using System;
using System.Collections.Concurrent;
using Main;
using Newtonsoft.Json;
using System.Collections.Generic;


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
                //this.Elo = Elo;

            }

             


            public double Base
            {
                get =>  Fans + Followers;
            }


        }



        [JsonProperty] private readonly ConcurrentDictionary<string, PopularityStruct> Popularity;

        FighterPopularity()
        {

            Popularity = new ConcurrentDictionary<string, PopularityStruct>();
        }

        public bool AddFighter(Fighter fighter)
        {
            PopularityStruct p = new PopularityStruct(100, 100, 1);
            return Popularity.TryAdd(fighter.Name, p);
        }

        public void AddFighters(IEnumerable<Fighter> fighters)
        {
            foreach (var fighter in fighters)
                AddFighter(fighter);
        }

        public double Coefficient(Fighter f) => Popularity[f.Name].Coefficient;

        public double Fans(Fighter f) => Popularity[f.Name].Fans;

        public double Followers(Fighter f) => Popularity[f.Name].Followers;

        public double Base(Fighter f) => Popularity[f.Name].Base;

        public (double fans, double followers, double coefficient) PopularityData(Fighter f)
        {

            PopularityStruct ps = Popularity[f.Name];

            return (fans: Popularity[f.Name].Fans, followers: Popularity[f.Name].Followers, coefficient: Popularity[f.Name].Coefficient);

        }


        public bool isEliteFighter(Fighter f)
        {

            return true;
        }


        public double CalculatePopularityChange(Fighter f1, Fighter f2, double score)
        {


            return 0;
        }


        public double FightViewers(Fighter f1, Fighter f2)
        {

            var popularity1 = PopularityData(f1);
            var popularity2 = PopularityData(f2);

            Console.WriteLine(popularity1.fans);



            return 0;
        }




    }
}
