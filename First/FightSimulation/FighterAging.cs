using System;
using System.Collections.Generic;
using System.Linq;

namespace FightSim
{
    public static class FighterAging
    {
        public static Dictionary<int, double> AGING_DICT;
        public const int ALWAYS_BAD_CURVE = 100000;
        public static (int AGE, double AGE_BUFF) MIN_AGE;
        public static (int AGE, double AGE_BUFF) MAX_AGE;


        static FighterAging()
        {
            AGING_DICT = new Dictionary<int, double>
            {
                {16, -26},   //75
                {17, -25},   //75
                {18, -24},   //75
                {19, -22},   //78
                {20, -19},   //81
                {21, -16},   //84
                {22, -12},   //88
                {23, -8},    //92
                {24, -6},    //94
                {25, -4},    //96
                {26, -2},    //98
                {27, 0},    //100
                {28, 0},    //100
                {29, 0},    //100
                {30, 0},    //100
                {31, 0},    //100
                {32, 0},    //100
                {33, -1},    //99
                {34, -2},    //98
                {35, -3},    //97
                {36, -3},    //97
                {37, -4},    //96
                {38, -5},    //95
                {39, -7},    //93
                {40, -10},   //90
                {41, -12},   //88
                {42, -14},   //86
                {43, -16},   //84
                {44, -18},   //82
                {45, -20},   //80
                {46, -23},   //77
                {47, -26},   //74
                {48, -29},   //71
                {49, -32},   //68
                {50, -37},   //63
                {51, -44},   //56
                {52, -50},   //56
            };

            MIN_AGE.AGE = AGING_DICT.Keys.Min();
            MIN_AGE.AGE_BUFF = AGING_DICT[MIN_AGE.AGE];

            MAX_AGE.AGE = AGING_DICT.Keys.Max();
            MAX_AGE.AGE_BUFF = AGING_DICT[MAX_AGE.AGE];
        }

        private static readonly Dictionary<string, int> AGE_IMPACTED_PROPERTIES =
            new Dictionary<string, int>
            {
                { "Accuracy",   0},
                { "Defense",    0},
                { "Durability", 0},
                { "FootWork",   0},
                { "HandSpeed", -2},
                { "Power",      1},
                { "Stamina",    0},

        ///     { "RingGen", 0}, Keep this as you age

            };

        public static double GetAgeBuff(int age, int curve)
        {
            age = Math.Max(MIN_AGE.AGE, age + curve);
            age = Math.Min(MAX_AGE.AGE, age);

            return AGING_DICT[age];
        }

        public static void YouthDebuff(Main.Fighter fighter)
        {
            if (fighter.AgeCurve == ALWAYS_BAD_CURVE) //bad fighters are already bad, do not debuff
                return;

            foreach(var _property in AGE_IMPACTED_PROPERTIES)
            {
                var property = fighter.GetType().GetProperty(_property.Key);
                var potentialSkill = (double) property.GetValue(fighter);
                double newSkill = potentialSkill - GetAgeBuff(fighter.Age, fighter.AgeCurve + _property.Value);

                newSkill = Math.Max(0,   newSkill);

                property.SetValue(fighter, newSkill, null);
            }
        }

        public static void BirthdayBuff(Main.Fighter fighter)
        {
            if (fighter.AgeCurve == ALWAYS_BAD_CURVE) //bad fighters are already shit, do not touch
                return;

            foreach (var _property in AGE_IMPACTED_PROPERTIES)
            {
                var property = fighter.GetType().GetProperty(_property.Key);
                var currentSkill = (double) property.GetValue(fighter);
                var lastYearBuff = GetAgeBuff(fighter.Age-1, fighter.AgeCurve + _property.Value);
                var thisYearBuff = GetAgeBuff(fighter.Age,   fighter.AgeCurve + _property.Value);

                double newSkill = currentSkill + (lastYearBuff - thisYearBuff);

                newSkill = Math.Max(0, newSkill);
                newSkill = Math.Min(100, newSkill);

                property.SetValue(fighter, newSkill, null);
            }
        }
    }
}
