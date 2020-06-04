using System;
namespace Boxing
{
    public class Country
    {
        public string Name { get; }              //Country Full Name - the identifier
        public string DisplayName { get; }       //GUI Friendly Short Name <= 10 characters
        public double PopularityBuff { get; }    //Does being from the country make you popular? 
        public double NationalityWeight { get; } //Probability a fighter is from this country

        public Country(string name, string displayName, double popularityBuff, double nationalityWeight)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.PopularityBuff = popularityBuff;
            this.NationalityWeight = nationalityWeight;

            if (displayName.Length > 10)
                throw new Exception("Display name for a country should not over 10 characters");
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
            return this.Name.GetHashCode();
        }
    }
}
