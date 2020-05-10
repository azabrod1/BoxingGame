using System;
namespace Fighting
{

    public enum Result
	{
        BEFORE_FIGHT = -1,
        FIGHTER1_WON = 1, 
        FIGHTER2_WON = 2,
        DRAW = 3,
        CANCELLED = 4,
        IN_PROGRESS = 5,
        OTHER = 10


    };

    public enum MethodOfResult
    {
        KO,
        TKO,
        UD,
        MD,
        NC, //No Contest
    }
}
