using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO
{
    public class ClassDto
    {
        public Guid? Id { get; set; }

        [Required]
        public int TimeSlotNumber { get; set; }

        [Required]
        public string ClusterNumber { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public Guid? LectureHallId { get; set; }

        public Guid? EducatorId { get; set; }

        public string? ClassTypeName { get; set; }
    }

    public class ClassEditDto
    {
        public Guid? Id { get; set; }

        public int? TimeSlotNumber { get; set; }

        public string? ClusterNumber { get; set; }

        public DateTime? Date { get; set; }

        public Guid? LectureHallId { get; set; }

        public Guid? EducatorId { get; set; }

        public string? ClassTypeName { get; set; }
    }
}
