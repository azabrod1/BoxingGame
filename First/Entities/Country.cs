
using System;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Main
{
    public class Country
    {

        public string Name { get; }              //Country Full Name - the identifier
        public string ShortName { get; }         //GUI Friendly Short Name <= 3 characters
        public double PopularityBuff { get; }    //Does being from the country make you popular? 
        public double Frequency { get; }         //Probability a fighter is from this country

        //country name -> country object
        public static Dictionary<string, Country> Countries { get; internal set; }
        public static double COUNTRY_FREQ_SUM;

        public Country(string name, string shortName, double popularityBuff, double nationalityFreq)
        {
            this.Name = name;
            this.ShortName = shortName;
            this.PopularityBuff = popularityBuff;
            this.Frequency = nationalityFreq;

            if (shortName.Length > 3)
                throw new Exception("Display name for a country should not over three characters");
        }

        static Country()
        {
            Countries = LoadCountriesOfTheWorld();
            COUNTRY_FREQ_SUM = Countries.Select(element => element.Value.Frequency).Sum();
        }

        public override bool Equals(Object obj)
        {
            // Perform an equality check on two rectangles (Point object pairs).
            if (obj == null || GetType() != obj.GetType())
                return false;

            Country that = (Country)obj;

            return (this.Name != null) && (that.Name != null) && this.Name.Equals(that.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            return Utility.UsefulString(this);
        }

        //So that Country w = "United States Of America" will work as expected
        public static implicit operator Country(string countryName)
        {
            return Get(countryName);
        }

        public static Country Get(string countryName)
        {

            return Countries[countryName];
        }

        public static List<Country> AllCountries()
        {
            return new List<Country>(Countries.Values);
        }

        private static Dictionary<string, Country> LoadCountriesOfTheWorld()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("FightConfig.xml"));
            XElement config = null;

            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (null == (config = XElement.Load(stream)))
                    throw new NullReferenceException("Unable to load FightConfig.xml");
            }

            Dictionary<string, Country> _countries = (from nation in config.Descendants("Country")

                                  select new
                                  {
                                      Name = nation.Element("Name").Value,
                                      ShortName = nation.Element("ShortName").Value,
                                      Frequency = double.Parse(nation.Element("Frequency").Value),
                                      Popularity = double.Parse(nation.Element("Popularity").Value),
                                  })
                                  .ToDictionary(
                                        structure => structure.Name,
                                        structure => new Country(structure.Name, structure.ShortName, structure.Popularity, structure.Frequency)
                                  );
            return _countries;
        }

        //According to the expected size of the weight class
        public static Country RandomNationality()
        {
            List<Country> countries = Country.AllCountries();
            double totalFreq = Country.COUNTRY_FREQ_SUM;

            double target = MathUtils.RangeUniform(0, totalFreq);
            double sum = 0;
            int c = -1;

            do
            {
                sum += countries[++c].Frequency;
            }
            while (sum < target);

            return countries[c];
        }

    }
}