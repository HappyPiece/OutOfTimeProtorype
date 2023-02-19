using Microsoft.AspNetCore.Mvc;

namespace OutOfTimePrototype.Dto
{
    public class ClassQueryDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        //public List<int> TimeSlotNumbers { get; set; } = new List<int>();
        public string? ClusterNumber { get; set; }
        public Guid? EducatorId { get; set; }
        public Guid? LectureHallId { get; set; }
        //public List<string> ClassTypeNames { get; set; } = new List<string>();
    }
}
