using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using static OutOfTimePrototype.Utilities.ClassUtilities;
using static OutOfTimePrototype.Utilities.ClassUtilities.ClassCreationResult;


namespace OutOfTimePrototype.Services;

public class ClassService : IClassService
{
    private readonly OutOfTimeDbContext _outOfTimeDbContext;

    public ClassService(OutOfTimeDbContext outOfTimeDbContext)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
    }

    public async Task<ClassCreationResult> TryCreateClass(ClassDTO classDTO)
    {
        // WARNING: cluster, probably as well as the educator, cannot be null
        var cluster = await _outOfTimeDbContext.Clusters.FindAsync(classDTO.ClusterNumber);
        var educator = await _outOfTimeDbContext.Educators.FindAsync(classDTO.EducatorId);

        var newClass = new Class
        {
            Cluster = cluster,
            Date = classDTO.Date,
            Educator = educator
        };

        // selects classes that share date and time slot with that being created
        var concurrentClasses = _outOfTimeDbContext.Classes
            .Where(x => x.TimeSlot.Number == newClass.TimeSlot.Number &&
                        DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(newClass.Date));

        if (await concurrentClasses.AnyAsync(x => x.Cluster.Number == newClass.Cluster.Number))
            return new ClassCreationResult(ClassCreationStatus.ClusterOccupied);

        if (newClass.Educator is not null && await concurrentClasses.Where(x => x.Educator != null)
                .AnyAsync(x => x.Educator!.Id == newClass.Educator.Id))
            return new ClassCreationResult(ClassCreationStatus.EducatorOccupied);

        if (newClass.LectureHall is not null && await concurrentClasses.Where(x => x.LectureHall != null)
                .AnyAsync(x => x.LectureHall!.Id == newClass.LectureHall.Id))
            return new ClassCreationResult(ClassCreationStatus.EducatorOccupied);

        return new ClassCreationResult();
    }
}