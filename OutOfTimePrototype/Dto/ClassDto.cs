using OutOfTimePrototype.DAL.Models;
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

        //public Guid? LectureHallId { get; set; }

        //public Guid? EducatorId { get; set; }

        //public Guid? SubjectId { get; set; }

        public LectureHallDto? LectureHall { get; set; }

        public CampusBuildingDto? CampusBuilding { get; set; }

        public EducatorDto? Educator { get; set; }

        public SubjectDto? Subject { get; set; }

        public string? ClassTypeName { get; set; }

        public void Format()
        {
            Date = Date.ToUniversalTime();
        }

        public ClassDto()
        {

        }

        public ClassDto (Class @class)
        {
            Id = @class.Id;
            Date = @class.Date;
            TimeSlotNumber = @class.TimeSlot.Number;
            ClusterNumber = @class.Cluster.Number;
            Educator = @class.Educator is not null ? new EducatorDto(@class.Educator) : null;
            LectureHall = @class.LectureHall is not null ? new LectureHallDto(@class.LectureHall) : null;
            CampusBuilding = @class.LectureHall is not null && @class.LectureHall.HostBuilding is not null ? new CampusBuildingDto(@class.LectureHall.HostBuilding) : null;
            Subject = @class.Subject is not null ? new SubjectDto(@class.Subject) : null;
            ClassTypeName = @class.Type?.Name;
        }
    }

    public class CreateClassDto : Utilities.IFormattable
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
