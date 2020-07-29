using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Main;
using Newtonsoft.Json;

namespace Main
{
    public enum PersonType
    {
        JUDGE,
        ALL
    }

    public abstract class Person
    {
        public PersonType Type { get; set; }
        public string Name { get; set; }    //Should be unique
        public Person(PersonType type, string name)
        {
            this.Type = type;
            this.Name = name;
        }
    }

    /* Note: not thread safe, do not modify while other threads are accessing
     * 
     * People Cache - hold all auxilary people/agents in the game
     * in one place i.e.s
     * 
     */

    [NewGame]
    [Serializable]
    public class PeopleCache
    {
        [JsonProperty] private Dictionary<string, Person> Map { get; set; }
        [JsonProperty] private List<Person>[] Data            { get; set; }

        public PeopleCache()
        {
            Map = new Dictionary<string, Person>();

            Data = new List<Person>[Enum.GetNames(typeof(PersonType)).Length - 1];

            for (int l = 0; l < Data.Length; ++l)
            {
                Data[l] = new List<Person>();
            }
        }

        public Person Get(string name)
        {
            return Map[name];
        }

        public bool Contains(string name)
        {
            return Map.ContainsKey(name);
        }

        public Person this[string name]
        {
            set
            {
                int typeNum = (int)value.Type;
                Map[name] = value;

                if (Map.ContainsKey(name))
                {
                    Data[typeNum][Data[typeNum].IndexOf(value)] = value;
                }
                else
                {
                    Data[typeNum].Add(value);
                }
            }

            get
            {
                return Map[name];
            }
        }

        public void Add(Person person)
        {
            Map[person.Name] = person;
        }

        public List<Person> GetAll(PersonType type = PersonType.ALL)
        {
            if (type == PersonType.ALL)
                Map.Values.ToList();

            return new List<Person>(Data[(int)type].ToList());
        }

        public int Count
        {
            get
            {
                return Map.Count;
            }
        }

        public ICollection<Person> RandomBunch(PersonType personType, int count)
        {
            return MathUtils.RandomBunch(Data[(int)personType], count);
        }

        //public ICollection<PType> RandomBunch<PType>( int count)
        //{

        //    return MathUtils.RandomBunch(Data[(int)personType], count);
        //}

        //public Fighter CreateRandomFighter(int weight = -1)
        //{
        //    Fighter hungryYoungLion = NewFighterWithRandomName();

        //    string[] ScaledCombatTraits =
        //    ConfigurationManager.AppSettings["ScaledFighterCombatTraits"].Split(",");

        //    if (weight == -1)
        //        weight = AssignWeightClass().Weight;

        //    hungryYoungLion.Weight = weight;
        //    hungryYoungLion.Nationality = Country.RandomNationality().Name;

        //    int totalSkillPoints = RandomSkillLevel() * ScaledCombatTraits.Count();

        //    foreach (string property in ScaledCombatTraits)
        //        hungryYoungLion.GetType().GetProperty(property).SetValue(hungryYoungLion, 0, null);

        //    while (totalSkillPoints > 0)
        //    {
        //        int toUpgrade = MathUtils.RangeUniform(0, ScaledCombatTraits.Count());
        //        var property = hungryYoungLion.GetType().GetProperty(ScaledCombatTraits[toUpgrade]);
        //        double currentSkill = (double)property.GetValue(hungryYoungLion);
        //        if (currentSkill < 100)
        //        {
        //            property.SetValue(hungryYoungLion, currentSkill + 1, null);
        //            --totalSkillPoints;
        //        }
        //    }

        //    string[] UniformCombatTraits =
        //        ConfigurationManager.AppSettings["UniformFighterCombatTraits"].Split(",");

        //    foreach (string property in UniformCombatTraits)
        //        hungryYoungLion.GetType().GetProperty(property).SetValue(hungryYoungLion, MathUtils.RangeUniform(0, 100), null);

        //    FighterRanking.FighterPopularity.UpdatePopularity(hungryYoungLion);

        //    return hungryYoungLion;
        //}

        //public Fighter NewFighter(string name, bool force = false)
        //{
        //    Fighter fighter = new Fighter(name);

        //    if (force)
        //    {
        //        Cache[name] = fighter;
        //        return fighter;
        //    }

        //    return Cache.TryAdd(fighter.Name, fighter) ? fighter : null;
        //}

        ////Assign the fighter a unique name - this will
        ////be the fighter's ID!! 
        //private Fighter NewFighterWithRandomName()
        //{
        //    Fighter fighter = null;
        //    bool preferCommonNames = Cache.Count < 10000; //Ensure common names proportionally represented

        //    for (int attempt = 0; attempt < 1000 && fighter == null; ++attempt)
        //    {
        //        string name = Utility.GetRandomFirstName(!preferCommonNames) + " " + Utility.GetRandomLastName(!preferCommonNames);
        //        fighter = NewFighter(name);
        //    }
        //    if (fighter == null)
        //        throw new InvalidOperationException("Fighter unique name generation failed");

        //    return fighter;
        //}





    }
}

