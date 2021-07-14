using System;

namespace webApp.Resources
{
    public class TimeRestrictionResource
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DayOfWeek Day { get; set; }
    }
}
