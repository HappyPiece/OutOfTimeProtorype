namespace OutOfTimePrototype.Utilities
{
    public static class ClassUtilities
    {
        public class ClassCreationResult
        {
            public ClassCreationStatus Status { get; set; }
            public string? ErrorMessage { get; set; }
            public ClassCreationResult(ClassCreationStatus classCreationStatus = ClassCreationStatus.Success, string? errorMessage = null)
            {
                Status = classCreationStatus;
                ErrorMessage = errorMessage;
            }

            public enum ClassCreationStatus
            {
                Success,
                ClusterOccupied,
                LectureHallOccupied,
                EducatorOccupied
            }
        }
    }
}
