using System;
using System.Collections.Generic;
using System.Linq;

namespace Main
{
    public static class MathUtils
    {

        [ThreadStatic] static readonly Random random = new Random();

        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + RangeUniform(0, n-i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        public static void Shuffle<T>(this List<T> values)
        {
            int n = values.Count();
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + RangeUniform(0, n - i);
                T t = values[r];
                values[r] = values[i];
                values[i] = t;
            }
        }


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
            double u1 = 1.0 - random.NextDouble(); //uniform(0,1]  ranom doubles
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
                Stds.Add(MathUtils.StandardDeviation(sample,false));
            }

            double finalAvg = Avgs.Average();
            double finalStd = MathUtils.StandardDeviation(Avgs,false);
            double elementStd = Stds.Average();

            return String.Format($"Avg {finalAvg} , Std {finalStd}, elementStd {elementStd}" );

        }

        public static double WeightedAverage(params int[] list)
        {
            if (list.Length == 0 || (list.Length & 1) == 1)
                return -1;

            int i = 0;
            double weightSum = 0;
            double weightedSum = 0;
            while (i < list.Length)
            {
                int element = list[i++];
                int weight = list[i++];
                weightSum += weight;
                weightedSum += element * weight;

            }

            return weightedSum / weightSum;

        }

        public static double StandardDeviation(this IEnumerable<double> values, bool populationStd = true)
        {
            double avg = values.Average();
            double std = Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
            if (!populationStd)
                std *= ((double)values.Count() / (values.Count() - 1.0));

            return std;
        }

        public static double WeightedAverage(params double[] list)
        {
            if (list.Length == 0 || (list.Length & 1) == 1)
                return -1;

            int i = 0;
            double weightSum = 0;
            double weightedSum = 0;
            while (i < list.Length)
            {
                double element = list[i++];
                double weight = list[i++];
                weightSum += weight;
                weightedSum += element * weight;

            }

            return weightedSum / weightSum;

        }

    }
}
