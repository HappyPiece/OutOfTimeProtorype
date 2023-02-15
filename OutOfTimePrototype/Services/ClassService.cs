using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;
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

        public async Task<ClassCreationResult> TryCreateClass(ClassDTO classDTO)
        {
            // TODO: maybe check for emptiness is needed
            var clusters = await _outOfTimeDbContext.Clusters
                .Where(c => classDTO.ClusterNumbers.Contains(c.Number))
                .ToListAsync();
            var educator = await _outOfTimeDbContext.Educators.SingleOrDefaultAsync(e => e.Id == classDTO.EducatorId);

            var newClass = new Class
            {
                Clusters = clusters,
                Date = classDTO.Date,
                Educator = educator
            };

            // selects classes that share date and time slot with that being created
            var concurrentClasses = _outOfTimeDbContext.Classes.Where(x => x.TimeSlot.Number == newClass.TimeSlot.Number
                                                                           && DateOnly.FromDateTime(x.Date) ==
                                                                           DateOnly.FromDateTime(newClass.Date));

            // checks if any of the concurrent classes has any cluster that intersects with newClass clusters
            if (await concurrentClasses.AnyAsync(cClass =>
                    cClass.Clusters.Any(cluster => newClass.Clusters.Contains(cluster))))
            {
                return new ClassCreationResult(ClassCreationStatus.ClusterOccupied);
            }

            // TODO: educator cannot be null so check for it above
            if (newClass.Educator is not null &&
                await concurrentClasses.AnyAsync(x => x.Educator != null && x.Educator.Id == newClass.Educator.Id))
            {
                return new ClassCreationResult(ClassCreationStatus.EducatorOccupied);
            }

            if (newClass.LectureHall is not null && await concurrentClasses
                    .AnyAsync(x => x.LectureHall != null && x.LectureHall.Id == newClass.LectureHall.Id))
            {
                return new ClassCreationResult(ClassCreationStatus.EducatorOccupied);
            }

            return new ClassCreationResult();
        }
    }
}