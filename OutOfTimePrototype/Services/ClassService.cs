using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using static OutOfTimePrototype.Utilities.ClassUtilities;
using static OutOfTimePrototype.Utilities.ClassUtilities.ClassCreationResult;


namespace OutOfTimePrototype.Services
{
    public class ClassService : IClassService
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        
        public ClassService(OutOfTimeDbContext outOfTimeDbContext)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
        }
        public async Task<ClassCreationResult> TryCreateClass(ClassDto classDto)
        {
            var newClass = new Class
            {
                Cluster = await _outOfTimeDbContext.Clusters.SingleOrDefaultAsync(x => x.Number == classDto.ClusterNumber),
                Date = classDto.Date,
                Educator = await _outOfTimeDbContext.Educators.SingleOrDefaultAsync(x => x.Id == classDto.EducatorId),

            };

            // selects classes that share date and time slot with that being created
            var concurrentClasses = _outOfTimeDbContext.Classes.Where(x => x.TimeSlot.Number == newClass.TimeSlot.Number
            && DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(newClass.Date));

            if (await concurrentClasses.AnyAsync(x => x.Cluster.Number == newClass.Cluster.Number))
            {
                return new ClassCreationResult(ClassCreationStatus.ClusterOccupied);
            }

            if ((newClass.Educator is not null) ? await concurrentClasses.Where(x => x.Educator != null).AnyAsync(x => x.Educator!.Id == newClass.Educator.Id) : false)
            {
                return new ClassCreationResult(ClassCreationStatus.EducatorOccupied);
            }

            if ((newClass.LectureHall is not null) ? await concurrentClasses.Where(x => x.LectureHall != null).AnyAsync(x => x.LectureHall!.Id == newClass.LectureHall.Id) : false)
            {
                return new ClassCreationResult(ClassCreationStatus.EducatorOccupied);
            }

            return new ClassCreationResult();
        }
    }
}
