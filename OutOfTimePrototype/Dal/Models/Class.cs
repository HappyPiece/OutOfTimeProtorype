using OutOfTimePrototype.Dal.Models;

namespace OutOfTimePrototype.DAL.Models
{
    public class Class
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ClassType? Type { get; set; }

        public TimeSlot TimeSlot { get; set; }

        public Cluster Cluster { get; set; }

        public DateTime Date { get; set; }

        public LectureHall? LectureHall { get; set; }

        public Educator? Educator { get; set; } 

        public Subject? Subject { get; set; }
    }
}
