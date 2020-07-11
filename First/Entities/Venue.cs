using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Main;



namespace Boxing.Entities
{
    public class Venue
    {

        public string Name { get; }              //Country Full Name - the identifier
        public string ShortName { get; }         //GUI Friendly Short Name <= 3 characters
        public double elasticity { get; }
        public double capacity { get; }

        public static Dictionary<string, Venue> Venues { get; internal set; }
        //public static double VENUE_FREQ_SUM;

        public Venue(string name, string displayName, double elasticity, double capacity)
        {

            this.Name = name;
            this.ShortName = displayName;

            
        }

        static Venue()
        {
            Venues = LoadVenues();
            //VENUE_FREQ_SUM = Venues.Select(element => element.Value.Frequency).Sum();
        }



        private static Dictionary<string, Venue> LoadVenues()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("FightConfig.xml"));
            XElement config = null;

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (null == (config = XElement.Load(stream)))
                    throw new NullReferenceException("Unable to load FightConfig.xml");
            }

            Dictionary<string, Venue> _venues = (from location in config.Descendants("Venue")
                                                      select new
                                                      {
                                                          Name = location.Element("Name").Value,
                                                          ShortName = location.Element("ShortName").Value,
                                                          Elasticity = double.Parse(location.Element("Elasticity").Value),
                                                          Capacity = double.Parse(location.Element("Capacity").Value),
                                                      })
                                  .ToDictionary(
                                        structure => structure.Name,
                                        structure => new Venue(structure.Name, structure.ShortName, structure.Elasticity, structure.Capacity)
                                  );
            return _venues;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            return Utility.UsefulString(this);
        }

        public static implicit operator Venue(string venueName)
        {
            return Get(venueName);
        }


        public static Venue Get(string venueName)
        {

            return Venues[venueName];
        }


        public static List<Venue> AllVenues()
        {
            return new List<Venue>(Venues.Values);
        }

    }
}
