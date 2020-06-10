using System;
using System.Collections.Generic;
using Main;

namespace Boxing.FighterRanking
{

    public class FighterPerformance
    {

        private List<IPerformanceCalculator> CalcList;
        private Dictionary<string, IPerformanceCalculator> CalcDict;
        private Dictionary<string, double> CalcValues;


        public FighterPerformance(params IPerformanceCalculator[] args)
        {
            foreach (var a in args)
            {
                CalcList.Add(a);
                CalcDict.Add(a.Type, a);
                CalcValues.Add(a.Type, a.Value);
            }
        }

        // return value based on performance type (e.g. Get("elo"))
        public double Get(string type) => CalcValues[type];

        // return value based on performance calc (e.g. Get(PerformanceCalculatorElo)
        public double Get(IPerformanceCalculator calc) => CalcValues[calc.Type];

        // update specific performance type (non-fight)
        public double Update(string type, Fighter me)
        {
            var calc = CalcDict[type];
            return (calc == null)? 0 : calc.Update(me);
        
        }

        // update specific performance type (after fight)
        public double Update(string type, Fighter me, Fighter other, double score)
        {
            var calc = CalcDict[type];
            return (calc == null) ? 0 : calc.Update(me, other, score);
        }

        // update all performance types (non-fight)
        public void Update(Fighter me)
        {
            foreach (var calc in CalcList)
            {
                Update(calc.Type, me);
            }
        }

        // update all performance types (after fight)
        public void Update(Fighter me, Fighter other, double score)
        {
            foreach (var calc in CalcList)
            {
                Update(calc.Type, me, other, score);
            }
        }

        // initialize specific performance type
        public double Init(string type, Fighter me)
        {
            var calc = CalcDict[type];
            return (calc == null) ? 0 : calc.Init(me);

        }

        // initialize all performance types
        public void Init(Fighter me)
        {
            foreach (var calc in CalcList)
            {
                Init(calc.Type, me);
            }
        }



    }
}
