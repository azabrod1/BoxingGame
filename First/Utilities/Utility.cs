using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Utilities;

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
        public static string GetRandomLastName(bool uniform = false)
        {
            if (fnames.Equals(default(StrAttDistribution)))
                fnames = LoadDistributionFile("LName.txt");

            return ResultFromDistribution(fnames, uniform);
        }


        public static string GetRandomFirstName(bool uniform = false)
        {
            if (fnames.Equals(default(StrAttDistribution)))
                fnames = LoadDistributionFile("FName.txt");

            return ResultFromDistribution(fnames, uniform);
        }

        public static string GetRandomName(bool uniform)
        {
            return string.Format("{0} {1}", GetRandomFirstName(uniform), GetRandomLastName(uniform));
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

        private static string ResultFromDistribution(StrAttDistribution d, bool uniform = false)
        {
            if (uniform) //Disregard underlying populatity of names, pretend its uniform
            {
                int nIdx = MathUtils.RangeUniform(0, d.Names.Count);
                return d.Names[nIdx];
            }

            double bound = d.HiLimit[^1];
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
                xTotal *= Math.Pow((double)param[i + 2], ((double)param[i]) / 10.0);
                yTotal *= Math.Pow((double)param[i + 2], ((double)param[i + 1]) / 10.0);
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

            if (name.Length == 0)
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

        public static OSPlatform GetOSPlatform()
        {

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return OSPlatform.Windows;

            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                return OSPlatform.OSX;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return OSPlatform.OSX;
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return OSPlatform.Linux;
                }
            }

            throw new Exception($"Unidentified OS {RuntimeInformation.OSArchitecture }");
        }

        public static void ArgumentNotNull(object? value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

       //     Type t;
            //t.ge


        }


        public static object GetMemberValue(MemberInfo member, object target)
        {
            ArgumentNotNull(member, nameof(member));
            ArgumentNotNull(target, nameof(target));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(target);
                case MemberTypes.Property:
                    try
                    {
                        return ((PropertyInfo)member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException e)
                    {
                        throw new ArgumentException($"MemberInfo '{nameof(member)}' has index parameters {e}");
                    }
                default:
                    throw new ArgumentException($"MemberInfo '{nameof(member)}' is not of type FieldInfo or PropertyInfo");
            }
        }

        public static void SetMemberValue(MemberInfo member, object target, object? value)
        {
            ArgumentNotNull(member, nameof(member));
            ArgumentNotNull(target, nameof(target));

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)member).SetValue(target, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)member).SetValue(target, value, null);
                    break;
                default:
                    throw new ArgumentException($"MemberInfo '{nameof(member)}' must be of type FieldInfo or PropertyInfo");
            }
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(object o)
        {
            if (o == null) return false;
            return (o is IDictionary &&
                     o.GetType().IsGenericType &&
                     (o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>))
                     || o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(ConcurrentDictionary<,>))));
        }

        public static bool IsDictionary(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }


    }
}
