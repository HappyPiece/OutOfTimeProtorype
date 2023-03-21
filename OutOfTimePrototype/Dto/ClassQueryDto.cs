using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OutOfTimePrototype.Dto
{
    public class ClassQueryDto : IValidatable
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TimeSlotNumber { get; set; }
        public string? ClusterNumber { get; set; }
        public Guid? EducatorId { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? LectureHallId { get; set; }
        public string? ClassTypeName { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public bool IgnoreClusterHierarchy { get; set; } = false;
        public ModelStateDictionary Validate()
        {
            throw new NotImplementedException();
        }

        public ModelStateDictionary ValidateAsForCreateClasses()
        {
            var result = new ModelStateDictionary();
            if (StartDate is null)
            {
                result.AddModelError("StartDate", "Property is required");
            }
            if (EndDate is null)
            {
                result.AddModelError("EndDate", "Property is required");
            }
            if (DayOfWeek is null)
            {
                result.AddModelError("DayOfWeek", "Property is required");
            }
            return result;
        }
    }
}
