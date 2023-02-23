using OutOfTimePrototype.Dal.Models;
using static OutOfTimePrototype.Utilities.UserUtilities;

namespace OutOfTimePrototype.Utilities
{
    public static class DayOfWeekExtensions
    {
        public static List<DateTime> GetDayOfWeekFromBetweenDates(this DayOfWeek? dayOfWeek, DateTime startDate, DateTime endDate)
        {
            var result = new List<DateTime>();
            DateTime currentDate = startDate;
            while (currentDate.DayOfWeek != dayOfWeek && currentDate <= endDate)
            {
                currentDate = currentDate.AddDays(1);
            }
            while (currentDate <= endDate)
            {
                result.Add(currentDate.ToUniversalTime());
                currentDate = currentDate.AddDays(7);
            }
            return result;
        }
        public static List<DateTime> GetDayOfWeekFromBetweenDates(this DayOfWeek dayOfWeek, DateTime startDate, DateTime endDate)
        {
            DayOfWeek? day = dayOfWeek;
            return day.GetDayOfWeekFromBetweenDates(startDate, endDate);
        }
        
    }

    public static class RoleExtensions
    {
        public static bool CanAssign(this Role role, Role assignRole)
        {
            if (RoleUtilities.AssignHierarchy.TryGetValue(role, out var canAssign))
            {
                if (canAssign.Contains(assignRole))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
