
using System;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml.Linq;


namespace Main
{
    public class City
    {
        public string Name { get; }
        public double Longitude { get; }
        public double Lattitude { get; }
        public string Country { get; }
        public string CountryCode { get; }

        public string Municipality { get; }
        public double Population { get; }

        public string State { get => (Country == "United States" ? Municipality : ""); }


        public static Dictionary<string, City> AllCities = new Dictionary< string, City>();
        public static Dictionary<string, List<City>> CountryCities = new Dictionary<string, List<City>>();
        public static Dictionary<string, double> CountryPopulation = new Dictionary<string, double>();



        public City(string name, double longitude, double lattitude, string country, string countryCode, string muni, double population)
        {
            this.Name = name;
            this.Longitude = longitude;
            this.Lattitude = lattitude;
            this.Country = country;
            this.CountryCode = countryCode;

            this.Municipality = muni;
            this.Population = population;
        }

        static City()
        {

            City _city;
            
            var assembly = Assembly.GetExecutingAssembly();

            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Worldcities.txt"));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var v = line.Split("\",\"");
                    line = line.Replace("\"", "");

                    string _name = v[1];

                    double _long = double.Parse(v[2]);
                    double _lat = double.Parse(v[3]);
                    string _country = v[4];
                    string _countryCode = v[6];

                    string _muni = v[7];
                    double _pop = v[9].Equals("")?1000.0: double.Parse(v[9]);
                    _city = new City(_name,_long, _lat ,_country, _countryCode, _muni, _pop);

                    //only add a city if it does not exist, or if existing city has less people
                    if (!AllCities.ContainsKey(_name) || (AllCities[_name].Population < _city.Population))
                    {
                        AllCities[_name] = _city;


                        if (!CountryCities.ContainsKey(_country)) {
                            CountryCities[_country] = new List<City>();
                            CountryPopulation[_country] = 0;
                        }
                        CountryCities[_country].Add(_city);
                        CountryPopulation[_country] += _city.Population;
                    }
                }


            }


        }

        public static List<City> GetCities()
        {
            return new List<City>(AllCities.Values);
        }

        public static List<City> GetCities(string country)
        {
            return CountryCities.GetValueOrDefault(country);
        }

        public static City Get(string name)
        {
            return AllCities[name];
        }

        

        public static City RandomCity(string country)
        {


            List<City> cities;

            if (!CountryCities.TryGetValue(country, out cities) || cities == null)
            {
                //todo just create a fake city
                City newCity = new City("Anytown_" +country , 0, 0, country, country.Substring(0, 3), "", 1000);
                AllCities.Add(newCity.Name, newCity);
                cities = new List<City>();
                cities.Add(newCity);
                CountryCities[country] = cities;
                CountryPopulation[country] = 1000;

                return newCity;
            }
                
            

            double random = MathUtils.RangeUniform(0.0, CountryPopulation[country]);
            double pop = 0;
            int idx = 0;
            while (random > pop)
            {
                
                pop += cities[idx++].Population;
            }

            return cities[idx-1];

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

        public static implicit operator City(string name)
        {
            return Get(name);
        }

        public static double GetDistance(string city1, string city2)
        {
            City c1 = city1;
            City c2 = city2;
            if (city1 == null || city2 == null)
                return -1;

            return GetDistance(c1.Longitude, c1.Lattitude, c2.Longitude, c2.Lattitude)/1000.0;

        }


        public static double LocationBuff(Fighter f, Venue v)
        {
            City vcity = (City)v.City;
            double buff = 1.0;

            double distance = GetDistance(f.HomeTown.Name, v.City);

            if (distance <= 120.0)
                buff = 1.2;

            if (PreferredCity(vcity, f.Nationality))
                buff *= 1.2;


            
            return buff;

        }

        public static bool PreferredCity(City city, Country country)
        {
            string cname = country.Name;


            if (city.Country == cname)
                return true;

            // currently only supported for the US
            if (city.Country != "United States")
                return false;

            string state = city.State;

            // by the US state
            switch (state)
            {
                case "New York":
                    return ((cname == "Puerto Rico") || (cname == "Poland"));
                        
                case "Nevada":
                case "Texas":
                case "California":
                case "Arizona":
                    return (cname == "Mexico");
                case "Illinois":
                    return (cname == "Poland");
                case "Florida":
                    return (cname == "Cuba");
                case "Pennsylvania":
                    return (cname == "Ireland");
                default:
                    return false;

            }

            




        }

        private static double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }


    }
}
