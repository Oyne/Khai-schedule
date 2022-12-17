using System.Collections.ObjectModel;

namespace Parsing
{
    public class WeekSchedule : IEnumerable<DaySchedule>
    {
        public DaySchedule Monday { get; }
        public DaySchedule Tuesday { get; }
        public DaySchedule Wednesday { get; }
        public DaySchedule Thursday { get; }
        public DaySchedule Friday { get; }
    }

    public class DaySchedule : IEnumerable<AlternateUniversityClass>
    {
        public ReadOnlyCollection<AlternateUniversityClass> Classes { get; }
    }

    public class UniversityClass
    {
        public string Name { get; }
        public string? RoomNumber { get; }
        public string? Type { get; }
        public string? Teacher { get; }
    }

    public class AlternateUniversityClass
    {
        public UniversityClass? Numerator { get; }
        public UniversityClass? Denominator { get; }

        public AlternateUniversityClass(UniversityClass? numerator, UniversityClass? denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }
}