using System;
using System.Collections.Generic;
using System.Linq;

namespace Main
{


    public static class StatsUtils
    {

        [ThreadStatic] static readonly Random random = new Random();


        public static int RangeUniform(int from, int to)
        {

            return random.Next(from, to);    
        }

        public static double RangeUniform(double from, double to)
        {
            return random.NextDouble() * (to - from) + from;
        }

        public static double Gauss(double μ = 0.5, double σ = 0.5)
        {
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         μ + σ * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }


        public static double Gauss2(double μ = 0.5, double σ = 0.5)
        {
            MathNet.Numerics.Distributions.Normal normalDist = new MathNet.Numerics.Distributions.Normal(0.5, 3);
            return normalDist.Sample();
        }


        public delegate double Dist();

        public static string RandomSample(int samples, int sampleSize, Dist f)
        {
            List<List<double>> allSamples = new List<List<double>>();

            for (int s = 1; s < samples; ++s)
            {
                List<double> sample = new List<double>();

                for (int e = 0; e < sampleSize; ++e)
                    sample.Add(f());

                allSamples.Add(sample);
            }

            List<double> Avgs = new List<double>();
            List<double> Stds = new List<double>();

            foreach(List<double> sample in allSamples){
                Avgs.Add(sample.Average());
                Stds.Add(Utility.StandardDeviation(sample,false));
            }

            double finalAvg = Avgs.Average();
            double finalStd = Utility.StandardDeviation(Avgs,false);
            double elementStd = Stds.Average();

            return String.Format($"Avg {finalAvg} , Std {finalStd}, elementStd {elementStd}" );

        }

    }
}
