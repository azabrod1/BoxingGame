using System;
namespace Main
{
    public static class Constants
    {

        public const double ATTRIB_BUFF_DEFAULT       = 2;
        public const double BLOCKS_IN_ROUND           = 3;


        //Math
        public const double CUBE_ROOT_TWO  = 1.25992104989;
        public const double SQR_ROOT_TWO   = 1.41421356237;
        public const double SQR_ROOT_THREE = 1.73205080757;

        //Figher Averages
        public const double JAB_RATIO_AVG       = 0.415;
        public const double JAB_RATIO_STD_RND   = 0.1;
        public const double JAB_MULTIPLIER_STD  = 0.30;

        //Fighting constants
        public const double AVG_WEIGHT_INV      = 1d/150d;
        public const double HAND_SPEED_ACC_BUFF = 0.25;
        public const double FOOT_WORK_ACC_BUFF  = 0.15;
        public const double JAB_POWER           = 0.15;
        public const double KNOCKDOWN_THRESHOLD = 7; //Lost too much health, its a knockdown!
        public const double P4P_BUFF_THRESHOLD  = 89; //Pound for pound fighter skills scale even faster, skills above it scale 2X as fast
        public const double WEIGHT_POWER_BUFF   = 3.5;
        public const double WEIGHT_DURABILITY_BUFF = 2.8;
        public const double PUNCHES_FROM_INTENSITY = 55;   //per fighter!
        public const double PUNCHES_THROWN_STD = 10;       //Math.Sqrt(2); //15 * Root(2); as we are adding two normals, we want std for punches thrown to be 15
        public const double PREF_DISTANCE_WEIGHT_ON_JAB = 0.70; //Jab mean offseted effected 70% by preferred distance, 30% actual distance


    }
}
