using System;
namespace Fighting
{

    public enum Result
	{
        BeforeFight = 0,
        Won = 1, // by first fighter
        Lost = 2,
        Draw = 3,
        Canceled = 4,
        InProgress = 5


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
