namespace MoleMole
{
    using System;

    public static class TimeUtil
    {
        private static int _dayTimeOffset;
        private static DateTime _lastCheckTime;
        private static TimeSpan _serverTimeSpan = TimeSpan.Zero;
        private static DateTime _testNow = DateTime.Parse("2016-03-03 12:00:00");
        private static bool debugTime = false;

        public static bool AcrossDailyUpdateTime()
        {
            DateTime time2 = _lastCheckTime.Date.AddSeconds((double) _dayTimeOffset).AddDays(1.0);
            return (Now >= time2);
        }

        private static string GetFormatTimeSplit(int num)
        {
            if (num > 9)
            {
                return num.ToString();
            }
            return ("0" + num);
        }

        public static string GetRemainTime(TimeSpan remainTime)
        {
            if (remainTime.Days > 0)
            {
                string[] textArray1 = new string[] { GetFormatTimeSplit(remainTime.Days), ":", GetFormatTimeSplit(remainTime.Hours), ":", GetFormatTimeSplit(remainTime.Minutes) };
                return string.Concat(textArray1);
            }
            return (GetFormatTimeSplit(remainTime.Hours) + ":" + GetFormatTimeSplit(remainTime.Minutes));
        }

        public static void SetDayTimeOffset(int seconds)
        {
            _dayTimeOffset = seconds;
            _lastCheckTime = Now;
        }

        public static void SetServerCurTime(uint timestamp)
        {
            _serverTimeSpan = (TimeSpan) (Miscs.GetDateTimeFromTimeStamp(timestamp) - Now);
        }

        public static DateTime DailyUpdateTime
        {
            get
            {
                return _lastCheckTime.Date.AddSeconds((double) _dayTimeOffset).AddDays(1.0);
            }
        }

        public static DateTime Now
        {
            get
            {
                if (debugTime)
                {
                    return _testNow;
                }
                return (DateTime.Now + _serverTimeSpan);
            }
        }
    }
}

