using System;
namespace Boxing
{
    [Serializable]
    public class GameData
    {
        private DateTime _StartDate;
        public int Week { get; internal set; }

        public GameData()
        {

        }

        public DateTime Today()
        {
            return _StartDate.Add(new TimeSpan(Week * 7, 0, 0, 0));
        }

        public void NewGame()
        {
            DateTime now = DateTime.Now;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Sunday)) % 7;
            _StartDate = now.AddDays(-1 * diff).Date;
        }
    }
}
