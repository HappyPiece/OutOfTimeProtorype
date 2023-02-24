using OutOfTimePrototype.Utilities;
using System.ComponentModel.DataAnnotations;

namespace OutOfTimePrototype.DTO
{
    public class ClassDto : Utilities.IFormattable
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

        public Guid? SubjectId { get; set; }

        public string? ClassTypeName { get; set; }

        public void Format()
        {
            Date = Date.ToUniversalTime();
        }
    }

    public class ClassEditDto : Utilities.IFormattable
    {
        public Guid? Id { get; set; }

        public int? TimeSlotNumber { get; set; }

        public string? ClusterNumber { get; set; }

        public DateTime? Date { get; set; }

        public Guid? LectureHallId { get; set; }

        public Guid? EducatorId { get; set; }

        public Guid? SubjectId { get; set; }

        public string? ClassTypeName { get; set; } 

        public DayOfWeek? DayOfWeek { get; set; }

        public void Format()
        {
            if (Date is not null)
            {
                DateTime date = Date ?? throw new ArgumentNullException();
                Date = date.ToUniversalTime();
            }
        }
    }
}
