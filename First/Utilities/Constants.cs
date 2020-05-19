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
        public const double FOOT_WORK_ACC_BUFF  = 0.1;
        public const double JAB_POWER           = 0.15;
        public const double KNOCKDOWN_THRESHOLD = 7; //Lost too much health, its a knockdown!


    }
}
