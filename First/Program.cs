using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FightSim;

using System.Xml.Linq;
using Boxing.FighterRating;
using Utilities;
using Newtonsoft.Json;
using log4net;
using log4net.Config;
using System.Reflection;

//using System.Text.Json;

namespace Main
{
    class Program
    {
        static readonly ILog LOGGER =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            //Console.WriteLine("****************************************\n\n");

            //Alex();
            //  Vlad(); //TODO Uncomment and comment out mine
            //  AlexConc();


            //  Json();
            // var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            // XmlConfigurator.Configure(logRepo, new FileInfo("app.config"));

            // XmlConfigurator.Configure();

            // WeightClass w = 147;
            // AlexConc();

            // Anya();

            //var age = FighterAging.AGING_DICT;

            //foreach(var x in age)
            //{
            //    Console.WriteLine("{" + $"{x.Key}, {100-x.Value}" +"},   " + $"//{x.Value}");
            //}

            //FighterCache boxers = new FighterCache();

            //Fighter f = boxers.CreateRandomFighter();
            //f.AgeCurve = 0;
            //FighterAging.YouthDebuff(f);

            //for (f.Age = 16 ; f.Age < 55; ++f.Age)
            //{
            //    FighterAging.BirthdayBuff(f);
            //    Console.WriteLine(f);
            //}

            //Application app = Application.CreateOrLoad();

            //app.SaveGame(); //Save Game

            Caroline();
        }

        static void Caroline()
        {
            Application app = Application.CreateOrLoad();

            app.SaveGame(); //Save Game

        }

        static void Anya()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            FightSimPlayTester game = new FightSimPlayTester();
            foreach (WeightClass wc in WeightClass.AllWeightClasses())
                game.AddFighters(wc.Size, wc.Weight);

            game.SimFights(50);

            Console.WriteLine(game.Status());

            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("elapsed {0}", elapsed_time);
        }

        static void Vlad()
        { 
        }

        static void AlexConc()
        {

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            const int A_Skill = 98;
            const int B_Skill = 93;

            Fighter fighter = new Fighter("Benji")
            {
                Weight = 147,
                Stamina = 100,
                HandSpeed = A_Skill,
                RingGen = A_Skill,
                Aggression = 50,
                FootWork = A_Skill,
                Reach = 84,
                Durability = A_Skill,
                Power = A_Skill,
                Defense = A_Skill,
                Accuracy = A_Skill,
            };

            Fighter opponent = new Fighter("Cody")
            {
                Weight = 147,
                Stamina = B_Skill,
                HandSpeed = B_Skill,
                RingGen = B_Skill,
                Aggression = 50,
                FootWork = B_Skill,
                Reach = 84,
                Durability = B_Skill,
                Power = 100,
                Defense = B_Skill,
                Accuracy = B_Skill,
            };

            List<FightOutcome> outcomes = new List<FightOutcome>();
            List<FightStats> fightStats = new List<FightStats>();

            List<Fight> fights = new List<Fight>();

            Fight fight = new Fight(fighter, opponent);

            for (int f = 0; f < 20000; ++f)
                fights.Add(fight);

            IFightSimulator fs = new FightSimulatorGauss();
            var results = fs.SimulateManyFightsWithDetails(fights);

            foreach (var (outcome, Stats) in results)
            {
                outcomes.Add(outcome);
                fightStats.Add(Stats.Condense());
            }

            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(fightStats.SummaryStats(fighter.Name, opponent.Name));
            Console.WriteLine(outcomes.SummaryFightOutcomes());

            //Console.WriteLine(fightStats.StandardDeviationDamage(true));
            //Console.WriteLine(fightStats.StandardDeviationDamage(false));

            Console.WriteLine("elapsed {0}", elapsed_time);
        }

        static void EloTest()
        {
            //Fighter foo = null;
            //for (int f = 0; f < 1000; ++f)
            //{
            //    foo = FighterCache.CreateRandomFighter();
            //    Console.WriteLine(foo);
            //};


            //     WeightclassConfig();
            //   WeightClass.LoadAllWeightClasses();

            IFighterRating elo = new EloFighterRating();
            FighterCache fc = new FighterCache();
            Fighter f1 = fc.CreateRandomFighter(147);
            Fighter f2 = fc.CreateRandomFighter(147);

            elo.AddFighter(f1); //This can be commented out, will auto add players
            elo.AddFighter(f2);

            //  Console.WriteLine(  elo.Rating(f1) );
            //  Console.WriteLine( elo.Rating(f2));

            FightSimulatorGauss fs = new FightSimulatorGauss();

            List<FightOutcome> list = new List<FightOutcome>();
            List<FightStats> stats = new List<FightStats>();

            for (int i = 0; i < 200; ++i)
            {
                var s = fs.SimulateFightWithDetails(new Fight(f1, f2));
                //Console.WriteLine(outcome.Winner);

                elo.CalculateRatingChange(f1, f2, s.outcome);
                list.Add(s.outcome);
                stats.Add(s.Stats.Condense());
                Console.WriteLine($"{elo.Rating(f1)}, {elo.Rating(f2)}, {s.outcome.WinnerNum()}");
            }

            //   Console.WriteLine(list.SummaryFightOutcomes());
            //    Console.WriteLine(stats.SummaryStats(f1.Name, f2.Name));

        }


        //This will be called on startup
        static Program()
        {

            //log4net.Util.LogLog.InternalDebugging = true;
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());

            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("log4net.config"));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                XmlConfigurator.Configure(logRepository, stream);
            }
        }

        static void Json()
        {
            string fileName = "anya3.json";

            FighterCache cache = new FighterCache();
            for (int s = 0; s < 100; ++s)
                cache.CreateRandomFighter(147);

            Fighter fighter = cache.CreateRandomFighter(147);
            cache.CreateRandomFighter(160);

            //var options = new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //};

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            string jsonString = JsonConvert.SerializeObject(cache, Formatting.Indented); //JsonSerializer.Serialize(cache, options);
            File.WriteAllText(fileName, jsonString);

            FighterCache reconstructed = JsonConvert.DeserializeObject<FighterCache>(jsonString); //JsonSerializer.Deserialize<FighterCache>(jsonString, options) ;

            //using (StreamWriter file = File.CreateText("Anya.json"))
            //{
            //    serializer.Serialize(file, movie);
            //}

            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;
            // Console.WriteLine(jsonString);

            Console.WriteLine("elapsed {0}", elapsed_time);

            Console.WriteLine("cache size {0}", reconstructed.Count());

            EloFighterRating e = new EloFighterRating(20);

            foreach (var f in cache.AllFighters())
            {
                e.AddFighter(f);
            }

            string se = JsonConvert.SerializeObject(e, Formatting.Indented); //JsonSerializer.Serialize(cache, options);

            Console.WriteLine(se);

            var re = JsonConvert.DeserializeObject<EloFighterRating>(se);

            Console.WriteLine(re.K);



        }

        static void PunchDistroTest(FightState fs)
        {
            Block block = new Block(fs);
            fs.Round = 3;

            block.Run();

            List<double> list = new List<double>();

            for (int i = 0; i < 1; ++i)
            {
                fs.Round = MathUtils.RangeUniform(1, 13);
                double x, y;
                x = y = 0;
                for (int min = 0; min < 1; ++min)
                {
                    block.Run();
                    var distro = block.CreatePunchSchedule();
                    foreach (var punch in distro)
                        Console.WriteLine(punch.Attacker);
                }

                list.Add(x + y);

                //Console.WriteLine("std {0}", list[list.Count - 1]);

                // list.Add(punches = (block.BoxerPunchesPerRound(normal) + block.BoxerPunchesPerRound(normal)));
                // Console.WriteLine("punches {0}",  punches);
            }

        }

        static void Alex()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            const int A_Skill = 85;
            const int B_Skill = 85;//;

            Fighter fighter = new Fighter("Benji")
            {
                Weight = 150,
                Stamina = A_Skill,
                HandSpeed = A_Skill,
                RingGen = A_Skill,
                Aggression = 50,
                FootWork = A_Skill,
                Reach = 84,
                Durability = A_Skill,
                Power = A_Skill,
                Defense = A_Skill,
                Accuracy = A_Skill,
            };

            Fighter opponent = new Fighter("Cody")
            {
                Weight = 150,
                Stamina = B_Skill,
                HandSpeed = B_Skill,
                RingGen = B_Skill,
                Aggression = 50,
                FootWork = B_Skill,
                Reach = 84,
                Durability = B_Skill,
                Power = B_Skill,
                Defense = B_Skill,
                Accuracy = B_Skill,
            };

            List<FightOutcome> outcomes = new List<FightOutcome>();
            List<FightStats> fightStats = new List<FightStats>();

            for (int f = 0; f < 1000; ++f)
            {
                Fight fight = new Fight(fighter, opponent);

                IFightSimulator fs = new FightSimulatorGauss();
                var result = fs.SimulateFightWithDetails(fight);

                //if(result.outcome.TimeOfStoppage == -1)
                Console.WriteLine(result.outcome);

                outcomes.Add(result.outcome);
                fightStats.Add(result.Stats.Condense());

                // Console.WriteLine();
                // Console.WriteLine("DAM {0}",result.Stats.AverageDamage(true));
            }

            stopwatch.Stop();
            long elapsed_time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(fightStats.SummaryStats(fighter.Name, opponent.Name));
            Console.WriteLine(outcomes.SummaryFightOutcomes());
            //Console.WriteLine(fightStats.StandardDeviationDamage(true));
            //Console.WriteLine(fightStats.StandardDeviationDamage(false));

            Console.WriteLine("elapsed {0}", elapsed_time);
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

            for (int i = 1; i < 2000; ++i)
            {
                state.Round = MathUtils.RangeUniform(1, 13);
                block.Run();
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
            block.Run();

            //Console.WriteLine(state.f2.PunchCapacity());



            // Console.WriteLine(block.BoxerPunchesPerRound(state.f1));

            List<double> f1 = new List<double>();
            List<double> f2 = new List<double>();

            for (int i = 1; i < 3000; ++i)
            {
                state.Round = MathUtils.RangeUniform(1, 13);
                block.Run();
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

            block.Run();

            List<double> list = new List<double>();

            for (int i = 1; i < 3000; ++i)
            {
                fs.Round = MathUtils.RangeUniform(1, 13);
                double x, y;
                x = y = 0;
                for (int min = 0; min < 3; ++min)
                {
                    block.Run();
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


            Console.WriteLine(Utility.GetRandomLastName());

            Dictionary<string, int> map = new Dictionary<string, int>();

            for (int i = 0; i < 10000; ++i)
            {
                string name = Utility.GetRandomLastName();
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

        private static void WeightclassConfig()
        {
            string filename = "FightConfig.xml";
            XElement res = new XElement("WeightClasses",
                              new XElement("WeightClass",
                                 new XAttribute("id", "LightFlyweight"),
                                 new XElement("MaxWeight", 108),
                                 new XElement("Popularity", 10),
                                 new XElement("Description", "Light Flyweight"),
                                 new XElement("Size", 500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Flyweight"),
                                 new XElement("MaxWeight", 112),
                                 new XElement("Popularity", 10),
                                 new XElement("Description", "Flyweight"),
                                 new XElement("Size", 500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "SuperFlyweight"),
                                 new XElement("MaxWeight", 115),
                                 new XElement("Popularity", 20),
                                 new XElement("Description", "Super Flyweight"),
                                 new XElement("Size", 500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Bantamweight"),
                                 new XElement("MaxWeight", 118),
                                 new XElement("Popularity", 20),
                                 new XElement("Description", "Bantamweight"),
                                 new XElement("Size", 500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "SuperBantamweight"),
                                 new XElement("MaxWeight", 122),
                                 new XElement("Popularity", 25),
                                 new XElement("Description", "Super Bantamweight"),
                                 new XElement("Size", 500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Featherweight"),
                                 new XElement("MaxWeight", 126),
                                 new XElement("Popularity", 30),
                                 new XElement("Description", "Featherweight"),
                                 new XElement("Size", 1000)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "SuperFeatherweight"),
                                 new XElement("MaxWeight", 130),
                                 new XElement("Popularity", 35),
                                 new XElement("Description", "Super Featherweight"),
                                 new XElement("Size", 1000)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Lightweight"),
                                 new XElement("MaxWeight", 135),
                                 new XElement("Popularity", 50),
                                 new XElement("Description", "Lightweight"),
                                 new XElement("Size", 1000)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "SuperLightweight"),
                                 new XElement("MaxWeight", 140),
                                 new XElement("Popularity", 70),
                                 new XElement("Description", "Super Lightweight"),
                                 new XElement("Size", 1500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Weltherweight"),
                                 new XElement("MaxWeight", 147),
                                 new XElement("Popularity", 100),
                                 new XElement("Description", "Weltherweight"),
                                 new XElement("Size", 2500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "SuperWeltherweight"),
                                 new XElement("MaxWeight", 154),
                                 new XElement("Popularity", 85),
                                 new XElement("Description", "Super Weltherweight"),
                                 new XElement("Size", 1750)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Middleweight"),
                                 new XElement("MaxWeight", 160),
                                 new XElement("Popularity", 90),
                                 new XElement("Description", "Middleweight"),
                                 new XElement("Size", 1750)
                              ),
                              new XElement("SuperMiddleweight",
                                 new XAttribute("id", "SuperMiddleweight"),
                                 new XElement("MaxWeight", 168),
                                 new XElement("Popularity", 65),
                                 new XElement("Description", "Super Middleweight"),
                                 new XElement("Size", 1500)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "LightHeavyweight"),
                                 new XElement("MaxWeight", 175),
                                 new XElement("Popularity", 80),
                                 new XElement("Description", "Light Heavyweight"),
                                 new XElement("Size", 1000)
                              ),
                              new XElement("WeightClass",
                                new XAttribute("id", "Cruiserweight"),
                                 new XElement("MaxWeight", 200),
                                 new XElement("Popularity", 40),
                                 new XElement("Description", "Cruiserweight"),
                                 new XElement("Size", 750)
                              ),
                              new XElement("WeightClass",
                                 new XAttribute("id", "Heavyweight"),
                                 new XElement("MaxWeight", -1),
                                 new XElement("Popularity", 135),
                                 new XElement("Description", "Heavyweight"),
                                 new XElement("Size", 1000)
                              )
                            );

            res.Save(filename);




        }

        public static void LoadCountries()
        {
            const string filename = "Countries.txt";
            HashSet<string> shorts = new HashSet<string>();
            Dictionary<string, Country> countries = new Dictionary<string, Country>();


            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

            XElement head = new XElement("Countries");
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                string _line = reader.ReadLine();
                string[] line = _line.Split(',');
                string country = line[0].Trim();
                string shortName = line[1].Trim();
                double freq = line.Length > 2 ? double.Parse(line[2]) : 30d;

                head.Add(new XElement("Country",
                                 new XAttribute("ID", country.Replace(" ", "")),
                                 new XElement("Name", country),
                                 new XElement("ShortName", shortName),
                                 new XElement("Popularity", 10),
                                 new XElement("Frequency", freq)
                 ));

            }

            head.Save("Countries.xml");

        }


    }
}
