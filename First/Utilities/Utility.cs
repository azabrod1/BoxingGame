using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Main
{
    public struct StrAttDistribution
    {

        public List<string> Names;
        public List<double> LowLimit;
        public List<double> HiLimit;

        public StrAttDistribution(List<string> names, List<double> lowLimit, List<double> hiLimit)
        {
            Names = names;
            LowLimit = lowLimit;
            HiLimit = hiLimit;
        }
    }

    public static class Utility
    {

        //Names
        static int PlayerNO = 1;
        public static string RandomNameSimple()
        {
            return String.Format("Fighter {0}", PlayerNO++);
        }

        public static void Print2DArray(int[,] arr)
        {
            var rowCount = arr.GetLength(0);
            var colCount = arr.GetLength(1);
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                    Console.Write(String.Format("{0}\t", arr[row, col]));
                Console.WriteLine();
            }
        }

        static StrAttDistribution fnames;
        public static string getRandomLastName()
        {
            if (fnames.Equals(default(StrAttDistribution)))
                fnames = LoadDistributionFile("LName.txt");

            return ResultFromDistribution(fnames);
        }


        public static string getRandomFirstName()
        {
            if (fnames.Equals(default(StrAttDistribution)))
                fnames = LoadDistributionFile("FName.txt");

            return ResultFromDistribution(fnames);
        }

        public static string getRandomName()
        {
            return string.Format("{0} {1}", getRandomFirstName(), getRandomLastName());
        }

        public static StrAttDistribution LoadDistributionFile(string filename)
        {
            List<string> names = new List<string>();
            List<double> lowLimit = new List<double>();
            List<double> hiLimit = new List<double>();

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    names.Add(values[0]);
                    lowLimit.Add(double.Parse(values[1]));
                    hiLimit.Add(double.Parse(values[2]));
                }
            }

            StrAttDistribution dist = new StrAttDistribution(names, lowLimit, hiLimit);
            return dist;
        }

        private static string ResultFromDistribution(StrAttDistribution d)
        {
            double bound = d.HiLimit[d.HiLimit.Count - 1];
            double target = MathUtils.RangeUniform(0d, bound);

            int lo = 0, hi = d.Names.Count - 1;

            while (lo <= hi)
            {
                int mid = hi + lo / 2;

                if (target < d.LowLimit[mid])
                    hi = mid - 1;
                else if (target > d.HiLimit[mid])
                    lo = mid + 1;
                else
                    return d.Names[mid];
            }

            return d.Names[lo];

        }


        public static double AttributeRatio(int x, int y, double attDif = Constants.ATTRIB_BUFF_DEFAULT)
        {
            double xx = Math.Pow(attDif, x / 10.0);
            double yy = Math.Pow(attDif, y / 10.0);

            return xx / (yy + xx);

        }

        public static double AttributeRatioCustom(params object[] param)
        {

            double xTotal = 1, yTotal = 1;

            if (param.Length == 0 || (param.Length % 3) != 0)
                return -1;

            int i = 0;

            while (i < param.Length)
            {
                xTotal *= Math.Pow((double)param[i + 2], ((int)param[i]) / 10.0);
                yTotal *= Math.Pow((double)param[i + 2], ((int)param[i + 1]) / 10.0);
                i += 3;
            }

            return xTotal / (yTotal + xTotal);
        }

        public static string UsefulString<T>(this IEnumerable<T> col)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in col)
                sb.AppendLine(UsefulString(item));

            return sb.ToString();
        }

        public static string UsefulString(Object obj)
        {
            if (obj == null)
                return "null";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            var _name = obj.GetType().GetProperty("Name");

            string name = (_name != null && _name.GetValue(obj) != null) ? _name.GetValue(obj).ToString() : "";

            if(name.Length == 0)
            {
                var _ID = obj.GetType().GetProperty("ID");
                if (_ID != null && _ID.GetValue(obj) != null)
                    name = _ID.GetValue(obj).ToString();
            }

            sb.AppendFormat($"{obj.GetType().Name}: { name } \n");
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string desc = descriptor.Name;
                object value = descriptor.GetValue(obj);
                sb.AppendFormat("{0}={1}\n", desc, value.ToString());
            }

            return sb.ToString();
        }

    }
}
