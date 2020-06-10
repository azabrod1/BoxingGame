using System;
using Main;
namespace Boxing.FighterRanking
{
    public interface IPerformanceCalculator
    {
        // returns name of the performance value
        public  string Type { get; }

        // returns current value (may return default value)
        public double Value { get; }

        // calculates value after a fight 
        public double Update(Fighter me, Fighter other, double score);

        // initializes value (may be fighter-specific)
        public double Init(Fighter me);

        // updates value between fights 
        public double Update(Fighter me);

    }
}
