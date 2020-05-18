using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FightSim;

namespace Main
{
    class Program
    {
        struct X
        {
            public List<int> list;
        }

        static void Main(string[] args)
        {

            Alex();
            Vlad(); //TODO Uncomment and comment out mine


        }

        static void Vlad()
        {
            FighterPool fp1 = new FighterPool();

            fp1.SimulateFights();

            Console.WriteLine(fp1.Stats());

            Console.WriteLine(fp1.Index("Fighter 594"));

            fp1.SimulateFight(1, 2);

        }

        static void Alex()
        {
            //FighterPool fp1 = new FighterPool();



            //TestRoundIntensity();
            //  double attR = Utility.AttributeRatioCustom(100, 90, 2.0, 70, 80, 2.0, 90, 80, 2.0, 50, 40, 2.0);


            //Console.WriteLine(attR);
            //TestLazyPussyFatSlowBum()

            //TestPreferredDistance();

            //    String result = StatsUtils.RandomSample( 10000, 10, () => StatsUtils.Gauss(40, 10) );
            //   Console.WriteLine(result);

            //TestJabPercent();

            //LogNormal();


            // AccuracyPower2();

            //  FuckingFight();
            /*
            Fighter lite = new Fighter();
            lite.Weight = 147;
            lite.Power = 80;
            FighterState L = new FighterState(lite);

            Fighter heavy = new Fighter();
            heavy.Weight = 160;
            heavy.Power = 50;
            FighterState H = new FighterState(heavy);


            Console.WriteLine("{0} {1}", L.Power(), H.Power() );*/


            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Console.WriteLine("wdef");
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            // TryKO();
            //stopwatch.Stop();
            //Console.WriteLine("{0}\n", stopwatch.ElapsedMilliseconds);

            const int A_Skill = 50;
            const int B_Skill = 50;

           Fighter fighter = new Fighter("Benji")
            {
                Weight     = 150,
                Stamina    = A_Skill,
                HandSpeed  = A_Skill,
                RingGen    = A_Skill,
                Aggression = A_Skill,
                FootWork   = A_Skill,
                Reach      = A_Skill,
                Durability = A_Skill,
                Power      = A_Skill,
                Defense    = A_Skill,

            };

            Fighter opponent = new Fighter("Cody")
            {
                Weight     = 150,
                Stamina    = B_Skill,
                HandSpeed  = B_Skill,
                RingGen    = B_Skill,
                Aggression = B_Skill,
                FootWork   = B_Skill,
                Reach      = B_Skill,
                Durability = B_Skill,
                Power      = B_Skill,
                Defense    = B_Skill,

            };

            List<FightOutcome> outcomes = new List<FightOutcome>();
            List<FightStats> fightStats = new List<FightStats>();

            for (int f = 0; f < 2000; ++f)
            {
                Fight fight = new Fight(fighter, opponent);

                FightSimulator fs = new FightSimulatorGauss();
                var result = fs.SimulateFightWithDetails(fight);

                //    Console.WriteLine(result.outcome);

                outcomes.Add(result.outcome);
                fightStats.Add(result.Stats.Condense());
            }

            Console.WriteLine(fightStats.SummaryStats(fighter.Name, opponent.Name));
            Console.WriteLine(outcomes.SummaryFightOutcomes());

            //  TryKO();
        }

        static void PunchDistroTest(FightState fs)
        {
            Block block = new Block(fs);
            fs.Round = 3;

            block.Play();

            List<double> list = new List<double>();

            for (int i = 0; i < 1; ++i)
            {
                fs.Round = MathUtils.RangeUniform(1, 13);
                double x, y;
                x = y = 0;
                for (int min = 0; min < 1; ++min)
                {
                    block.Play();
                    var distro = block.GeneratePunchDistribution();
                    foreach(var punch in distro)
                        Console.WriteLine(punch.Item2.Name());

                }


                list.Add(x + y);

                //Console.WriteLine("std {0}", list[list.Count - 1]);

                // list.Add(punches = (block.BoxerPunchesPerRound(normal) + block.BoxerPunchesPerRound(normal)));
                // Console.WriteLine("punches {0}",  punches);
            }

        }

        void Profile()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                MathUtils.Gauss2(0.5, 3);
            }
            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(elapsed_time);
        }

        const int PUNCHES = 13;
        const int FIGHTS = 10000; //ree

        public delegate double[] BlockOutcome(FighterState figher);

        public static void TryKO()
        {
            Fighter fighter = new Fighter
            {
                Weight = 150,
                Stamina = 50,
                HandSpeed = 50,
                RingGen = 95,
                Aggression = 50,
                FootWork = 50,
                Reach = 84,
                Durability = 50,
                Power = 50
            };

            Fighter opponent = new Fighter();
            opponent.Weight = 150;
            opponent.Stamina = 50;
            opponent.HandSpeed = 50;
            opponent.RingGen = 95;
            opponent.Aggression = 50;
            opponent.FootWork = 50;
            opponent.Reach = 84;
            opponent.Durability = 50;

            BoxScore[] fightResults = new BoxScore[FIGHTS];

            for (int f = 0; f < FIGHTS; ++f)
                fightResults[f] = SimFight(new FighterState(fighter), new FighterState(opponent), NormalCorr);

            Console.WriteLine(resultsSummary(fightResults, true));

        }

        public static double[] NormalCorr(FighterState fighter)
        {

            double blockDamage = Math.Max(0.5, MathUtils.Gauss(1, 0.75)); //How damaging will we be in this block?

            double[] results = new double[PUNCHES];

            for (int i = 0; i < PUNCHES; ++i)
            {

                double power = Math.Max(MathUtils.Gauss(1, 0.75), 1) * fighter.Power();

                double _acc = -0.28 - 0d * 0.01;
                double accuracy = Math.Max(0, MathUtils.Gauss(_acc, 0.5));
                //Console.WriteLine(accuracy);
                results[i] = Math.Max(power * accuracy, 0) * blockDamage;


            }

            MathUtils.Shuffle(results);


            double[] result = new double[] { results.Sum(), results.Where(d => d > 0).Count() / (double)PUNCHES };

            return result;
        }
        static Stopwatch stopwatch = new Stopwatch();

        public static double[] NormalCorrOld(FighterState fighter)
        {
            double blockDamage = Math.Max(0.5, MathUtils.Gauss(1, 0.75)); //How damaging will we be in this block?

            double[] results = new double[PUNCHES];

            for (int i = 0; i < PUNCHES; ++i)
            {
                double power = Math.Max(MathUtils.Gauss(1, 0.75), 1) * fighter.Power();
                double _acc = -0.33 + 0d * 0.01;
                double accuracy = Math.Max(0, MathUtils.Gauss(_acc, 0.6));


                //Console.WriteLine(accuracy);
                results[i] = Math.Max(power * accuracy, 0) * blockDamage * 1.4;
            }

            double[] result = new double[] { results.Sum(), results.Where(d => d > 0).Count() / (double)PUNCHES };

            // Console.WriteLine( string.Join(",", result));

            return result;
        }

        public static double[] NormalBlock()
        {
            double blockDamage = Math.Max(0.5, MathUtils.Gauss(1, 1.5)) * 4; //How damaging will I be in this block
            double blockAccuracy = Math.Max(0, MathUtils.Gauss(0.33, 0.07));

            double[] result = new double[] { blockAccuracy * blockDamage * PUNCHES, blockAccuracy };

            //  Console.WriteLine( string.Join(",", result));

            return result;
        }

        static string resultsSummary(BoxScore[] results, bool detailed = false)
        {
            int KO_Percent = (int)(100 * results.Where(r => r.Result < 36).Count() / (double)results.Count());
            double AvgDamage = (int)results.Select(r => r.AvgDamage()).Average();
            double AvgPercentLanded = (int)100 * results.Select(r => r.LandedPercent()).Average();

            System.Text.StringBuilder summary = new System.Text.StringBuilder("", 50);

            if (detailed)
                foreach (BoxScore fr in results)
                    summary.Append(fr + "\n");

            summary.AppendFormat($"KO_Percent: {KO_Percent} Landed % {AvgPercentLanded}, Avg Damage: {AvgDamage}");
            return summary.ToString();
        }


        static BoxScore SimFight(FighterState fighter, FighterState opponent, BlockOutcome F, int punchesPerBlock = PUNCHES)
        {
            BoxScore fightStats = new BoxScore();

            double[] damage = new double[36];
            double[] hits = new double[36];
            int result = 0;

            double rndDam = 0;

            for (int round = 0; round < 12; ++round)
            {
                rndDam = 0;
                for (int min = 0; min < 3; ++min)
                {
                    double[] outcome = F(fighter);
                    fightStats.AppendData(round, outcome[0], PUNCHES, PUNCHES * outcome[1]);

                    rndDam += damage[result] = outcome[0];
                    hits[result] = outcome[1];

                    if (opponent.IncrementHealth(-damage[result]) <= 0)
                    {
                        round = 12;
                        break;
                    }

                    opponent.RecoverFor(60);
                    ++result;

                }

                //   Console.WriteLine("Damage " + (int)rndDam);

                opponent.RecoverFor(60);
            }

            fightStats.Result = result;

            return fightStats;

        }

        static void AccuracyPower()
        {
            const int samples = 12;

            double sum = 0;

            for (int x = 0; x < samples; ++x)
            {
                Console.WriteLine();
                int S = 30;

                double[] results = new double[S];
                for (int i = 0; i < S; ++i)
                {
                    double power = Math.Max(MathUtils.Gauss(1, 1), 1);// *Math.Max(StatsUtils.Gauss(1, 1), 1);// * Math.Max(StatsUtils.Gauss(1, 10), 1);
                    double accuracy = Math.Max(0, MathUtils.Gauss(-0.3, 1));
                    results[i] = Math.Max(power * accuracy, 0) * 10;
                }

                Array.Sort(results);

                for (int i = S - S; i < S; ++i)
                    if (false)
                        Console.WriteLine($"{i} {results[i]}");

                sum += results.Sum();

                Console.WriteLine($"Sum {(int)results.Sum()}");
                Console.WriteLine($"Percent > 0 {(int)(100 * results.Where(x => x > 0).Count() / (double)S)}");
                Console.WriteLine($"Largest {results.Max()}");


                //  results.ToList().ForEach(i => Console.WriteLine(i.ToString()));

            }

            Console.WriteLine($"Sum {sum}");

        }

        static void AccuracyPower2()
        {
            const int samples = 12;

            double sum = 0;

            for (int x = 0; x < samples; ++x)
            {
                Console.WriteLine();
                int PUNCHES = 30;

                double[] results = new double[PUNCHES];
                double[] blockMult = new double[4];

                for (int b = 0; b < 4; ++b)
                    blockMult[b] = MathUtils.Gauss(1, 1);

                for (int i = 0; i < PUNCHES; ++i)
                {
                    double power = Math.Max(MathUtils.Gauss(1, 1), 1);// *Math.Max(StatsUtils.Gauss(1, 1), 1);// * Math.Max(StatsUtils.Gauss(1, 10), 1);
                    double accuracy = Math.Max(0, MathUtils.Gauss(-0.3, 1)) * blockMult[i * 4 / PUNCHES];
                    results[i] = Math.Max(power * accuracy, 0) * 10;
                }

                Array.Sort(results);

                for (int i = PUNCHES - PUNCHES; i < PUNCHES; ++i)
                    if (false)
                        Console.WriteLine($"{i} {results[i]}");

                sum += results.Sum();

                Console.WriteLine($"Sum {(int)results.Sum()}");
                Console.WriteLine($"Percent > 0 {(int)(100 * results.Where(x => x > 0).Count() / (double)PUNCHES)}");
                Console.WriteLine($"Largest {results.Max()}");


                //  results.ToList().ForEach(i => Console.WriteLine(i.ToString()));

            }

            Console.WriteLine($"Sum {sum}");

        }

        static void NormalSquared()
        {

            const int samples = 30;

            for (int x = 0; x < samples; ++x)
            {
                Console.WriteLine();
                int S = 50;

                double[] results = new double[S];
                for (int i = 0; i < S; ++i)
                    results[i] = (Math.Abs(MathUtils.Gauss(0, 12) * MathUtils.Gauss(0, 12)) - 100);

                //Array.Sort(results);

                //for (int i = S - S; i < S; ++i)
                //  Console.WriteLine($"{i} {results[i]}");

                Console.WriteLine($"Sum {(int)results.Sum()}");
                Console.WriteLine($"Percent > 1 {results.Where(x => x > 1).Count() / (double)S}");
                Console.WriteLine($"Largest {results.Max()}");


                //  results.ToList().ForEach(i => Console.WriteLine(i.ToString()));

            }

        }

        static void LogNormal()
        {
            const int samples = 12;

            for (int x = 0; x < samples; ++x)
            {
                Console.WriteLine();
                int S = 30;

                double[] results = new double[S];
                for (int i = 0; i < S; ++i)
                    results[i] = Math.Pow(1.7, MathUtils.Gauss(0.5, 2)) - 1;

                //Array.Sort(results);

                // for (int i = S - S; i < S; ++i)
                //    Console.WriteLine($"{i} {results[i]}");

                Console.WriteLine($"Sum {(int)results.Sum()}");
                Console.WriteLine($"Percent > 1 {(int)(100 * results.Where(x => x > 1).Count() / (double)S)}");
                Console.WriteLine($"Largest {results.Max()}");


                //  results.ToList().ForEach(i => Console.WriteLine(i.ToString()));

            }
        }

        static void TestLazyPussyFatSlowBum()
        {
            //Bum Fighter
            Fighter bum = new Fighter();

            bum.Weight = 250;
            bum.Stamina = 30;
            bum.HandSpeed = 30;
            bum.RingGen = 2;
            bum.Aggression = 30;

            Console.WriteLine("Fat Bum Fighter");
            TotalPunchTest(new FightState(new Fight(bum, bum)));

            // Console.BackgroundColor = ConsoleColor.Blue;
            // Console.WriteLine("dds {0}", normal.PunchCapacity(fs));

        }


        static void TestJabPercent()
        {
            //Normal Fighter
            Fighter normal = new Fighter();


            normal.Weight = 150;
            normal.Stamina = 50;
            normal.HandSpeed = 50;
            normal.RingGen = 95;
            normal.Aggression = 50;
            normal.FootWork = 90;
            normal.Reach = 84;


            Fighter normalAgg = new Fighter();

            normalAgg.Weight = 150;
            normalAgg.Stamina = 50;
            normalAgg.HandSpeed = 50;
            normalAgg.RingGen = 75;
            normalAgg.Aggression = 50;
            normalAgg.FootWork = 90;
            normalAgg.Reach = 80;

            Fight fight = new Fight(normal, normalAgg, 12);
            FightState state = new FightState(fight);
            Block block = new Block(state);

            List<double> list1 = new List<double>();
            List<double> list2 = new List<double>();

            for (int i = 1; i < 200; ++i)
            {
                state.Round = MathUtils.RangeUniform(1, 13);
                block.Play();
                // double r =block.RoundIntensity();
                //list.Add(normal.PunchCapacity(fs));

                double f1Jab = block.JabPercentages(state.F1, state.F2);
                double f2Jab = block.JabPercentages(state.F2, state.F1);

                Console.WriteLine("f1  {0}, f2 std {1}", (int)(f1Jab * 100), (int)(f2Jab * 100));

                list1.Add(f1Jab);
                list2.Add(f2Jab);


                // list.Add(punches = (block.BoxerPunchesPerRound(normal) + block.BoxerPunchesPerRound(normal)));
                // Console.WriteLine("punches {0}",  punches);
            }

            Console.WriteLine("f1 avg {0}, f1 std {1}", list1.Average(), list1.StandardDeviation());
            Console.WriteLine("f2 avg {0}, f2 std {1}", list2.Average(), list2.StandardDeviation());


        }


        static void TestPreferredDistance()
        {
            //Normal Fighter
            Fighter normal = new Fighter();


            normal.Weight = 150;
            normal.Stamina = 50;
            normal.HandSpeed = 50;
            normal.RingGen = 95;
            normal.Aggression = 50;
            normal.FootWork = 90;
            normal.Reach = 82;


            Fighter normalAgg = new Fighter();


            normalAgg.Weight = 150;
            normalAgg.Stamina = 50;
            normalAgg.HandSpeed = 50;
            normalAgg.RingGen = 75;
            normalAgg.Aggression = 50;
            normalAgg.FootWork = 90;
            normalAgg.Reach = 76;

            Fight fight = new Fight(normal, normalAgg, 12);
            FightState state = new FightState(fight);
            state.Round = 4;

            double distancePref1 = state.F1.PreferredDistance(state.F2);
            Console.WriteLine(distancePref1);

            double distancePref2 = state.F2.PreferredDistance(state.F1);
            Console.WriteLine(distancePref2);

            Console.WriteLine(state.FightDistance);
            //.WriteLine(state.fightControl);
            //  Console.WriteLine(state.ReachBuff());
            Block block = new Block(state);
            block.Play();

            //Console.WriteLine(state.f2.PunchCapacity());



            // Console.WriteLine(block.BoxerPunchesPerRound(state.f1));

            List<double> f1 = new List<double>();
            List<double> f2 = new List<double>();

            for (int i = 1; i < 3000; ++i)
            {
                state.Round = MathUtils.RangeUniform(1, 13);
                block.Play();
                // double r =block.RoundIntensity();
                //list.Add(normal.PunchCapacity(fs));


                var x = block.BoxerPunchesPerRound(state.F1);
                double y = block.BoxerPunchesPerRound(state.F2);

                f1.Add(x);
                f2.Add(y);

                //  Console.WriteLine("std {0}", list[list.Count - 1]);

                // list.Add(punches = (block.BoxerPunchesPerRound(normal) + block.BoxerPunchesPerRound(normal)));
                // Console.WriteLine("punches {0}",  punches);
            }


            Console.WriteLine("std b {0}", MathUtils.StandardDeviation(f1));
            Console.WriteLine("avg b {0}", f1.Average());

            Console.WriteLine("std b {0}", MathUtils.StandardDeviation(f2));
            Console.WriteLine("avg b {0}", f2.Average());

        }


        static void TestRoundIntensity()
        {
            //Normal Fighter
            Fighter normal = new Fighter();


            normal.Weight = 150;
            normal.Stamina = 50;
            normal.HandSpeed = 50;
            normal.RingGen = 50;
            normal.Aggression = 50;
            normal.FootWork = 70;

            Fighter normalAgg = new Fighter();


            normalAgg.Weight = 150;
            normalAgg.Stamina = 50;
            normalAgg.HandSpeed = 50;
            normalAgg.RingGen = 70;
            normalAgg.Aggression = 70;
            normalAgg.FootWork = 70;

            Fight fight = new Fight(normal, normalAgg, 12);
            FightState state = new FightState(fight);
            state.Round = 4;
            Block block = new Block(state);
            Console.WriteLine(block.CalcBlockIntensity());

        }

        static void TotalPunchTest(FightState fs)
        {
            Block block = new Block(fs);
            fs.Round = 3;

            block.Play();

            List<double> list = new List<double>();

            for (int i = 1; i < 3000; ++i)
            {
                fs.Round = MathUtils.RangeUniform(1, 13);
                double x, y;
                x = y = 0;
                for (int min = 0; min < 3; ++min)
                {
                    block.Play();
                    x += block.BoxerPunchesPerRound(fs.F1) / 3;
                    y += block.BoxerPunchesPerRound(fs.F2) / 3;
                }

                list.Add(x + y);

                //Console.WriteLine("std {0}", list[list.Count - 1]);

                // list.Add(punches = (block.BoxerPunchesPerRound(normal) + block.BoxerPunchesPerRound(normal)));
                // Console.WriteLine("punches {0}",  punches);
            }

            double std = MathUtils.StandardDeviation(list);

            Console.WriteLine("std {0}", std);
            Console.WriteLine("avg {0}", list.Average());

        }



        static void MaeeRin(string[] args)
        {
            /*
            int [] bucket = new int[400];


            for (int i = 0; i < 1000000; ++i)
            {
                double result = RandomUtils.Gauss(0, 1);
                //Console.WriteLine("Random number {0}", (int)(result * 10) + 200);

                bucket[ (int)(result * 10) + 200]++;
            }
            */
            string filePaths = System.IO.Directory.GetParent(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).ToString();
            filePaths = System.IO.Directory.GetParent(filePaths).ToString();
            filePaths = System.IO.Directory.GetParent(filePaths).ToString();


            DirectoryInfo d = new DirectoryInfo(filePaths);//Assuming Test is your Folder
            string[] Files = Directory.GetFiles(filePaths, "*.*", SearchOption.AllDirectories); //Getting Text files
            /*
            string str = "";
            foreach (FileInfo file in Files)
            {
                str = str + ", " + file.Name;
            }*/

            string[] fileArray = Directory.GetFiles(filePaths);

            Console.WriteLine(filePaths);

            Array.ForEach(Files, Console.WriteLine);


            Console.WriteLine(Utility.getRandomLastName());

            Dictionary<string, int> map = new Dictionary<string, int>();

            for (int i = 0; i < 10000; ++i)
            {
                string name = Utility.getRandomLastName();
                if (!map.ContainsKey(name))
                    map[name] = 1;
                else
                    map[name]++;
            }


            var myList = map.ToList();

            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            foreach (var i in myList)
            {
                Console.WriteLine(i);
            }



        }
    }
}
