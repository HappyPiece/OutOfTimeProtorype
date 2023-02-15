using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO
{
    public class ClassDTO
    {
        public Guid? Id { get; set; }

        [Required]
        public int TimeSlotNumber { get; set; }

        [Required]
        public string[] ClusterNumbers { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public Guid? LectureHallId { get; set; }

        public Guid? EducatorId { get; set; }

        public string? ClassTypeName { get; set; }
    }
}
