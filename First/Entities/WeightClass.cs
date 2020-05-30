using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Main
{
    public class WeightClass
    {
        public int Weight { get; }          //Weight limit
        public int Size { get; }            //Approx how many fighters we expect in the weight class
        public string Description { get; }  //How to display the Division Name for UI
        public string ID { get;}            //ID

        public static int WC_SIZE_SUM { get; }

        private static Dictionary<string, WeightClass> WeightClasses { get; }

        public WeightClass(int weight, int size, string description, string id)
        {
            this.Weight = weight;
            this.Size = size;
            this.Description = description;
            this.ID = id;
        }

        static WeightClass() {
            WeightClasses = LoadAllWeightClasses();
            WC_SIZE_SUM = WeightClasses.Select(element => element.Value.Size).Sum();
        }

        public static WeightClass Get(string weight)
        {
            return WeightClasses[weight];
        }

        public static WeightClass Get(int weight)
        {
            foreach (var entry in WeightClasses)
                if (entry.Value.Weight == weight)
                    return entry.Value;

            return null;
        }

        public static List<WeightClass> AllWeightClasses()
        {
            return new List<WeightClass>(WeightClasses.Values);
        }

        private static Dictionary<string, WeightClass> LoadAllWeightClasses()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("FightConfig.xml"));
            XElement config = null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (null == (config = XElement.Load(stream)))
                    throw new NullReferenceException("Unable to load XML fight config");
            }

            Dictionary<string, WeightClass> weightClasses = new Dictionary<string, WeightClass>(17);

            var _WeightClasses = (from wcs in config.Descendants("WeightClass")
                                  select new
                                  {
                                      ID = wcs.Attribute("id").Value,
                                      Weight = int.Parse(wcs.Element("MaxWeight").Value),
                                      Description = wcs.Element("Description").Value,
                                      Popularity = int.Parse(wcs.Element("Popularity").Value),
                                      Size = int.Parse(wcs.Element("Size").Value),

                                  }).ToList();

            foreach (var _wc in _WeightClasses)
            {
                weightClasses.Add(_wc.ID, new WeightClass(_wc.Weight, _wc.Size, _wc.Description, _wc.ID));
            };
            return weightClasses;
        }

        public override string ToString()
        {
            return Utility.UsefulString(this);
        }

    }
}
