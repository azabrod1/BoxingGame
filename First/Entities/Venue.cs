using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Main;



namespace Entities
{
    public class Venue
    {

        public string Name { get; }              //Country Full Name - the identifier
        //public string ShortName { get; }         //GUI Friendly Short Name <= 3 characters
        public double Elasticity { get; }
        public double Capacity { get; }
        public string City { get; }
        public string State { get; }

        public static Dictionary<string, Venue> Venues { get; internal set; }
        //public static double VENUE_FREQ_SUM;

        public Venue(string name, double elasticity, double capacity, string city, string state)
        {

            this.Name = name;
            this.Elasticity = elasticity;
            this.Capacity = capacity;
            this.City = city;
            this.State = state;
            //this.ShortName = displayName;

            
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

            //Console.WriteLine("In LoadVenues...");

            Dictionary<string, Venue> _venues = (from v in config.Descendants("Venue")
                                                      select new
                                                      {
                                                          Name = v.Element("Name").Value,
                                                          Elasticity = double.Parse(v.Element("Elasticity").Value),
                                                          Capacity = double.Parse(v.Element("Capacity").Value),
                                                          City = v.Element("City").Value,
                                                          State = v.Element("State").Value
                                                      })
                                  .ToDictionary(
                                        structure => structure.Name,
                                        structure => new Venue(structure.Name, structure.Elasticity, structure.Capacity,
                                                structure.City, structure.State)
                                  );
            //Console.WriteLine("Number of venues = " + _venues.Count);

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
