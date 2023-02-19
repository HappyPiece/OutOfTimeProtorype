using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.DTO;
using System.Diagnostics.Metrics;
using static OutOfTimePrototype.Utilities.ClassUtilities;
using static OutOfTimePrototype.Utilities.ClassUtilities.ClassOperationResult;


namespace OutOfTimePrototype.Services
{
    public class ClassService : IClassService
    {
        private readonly OutOfTimeDbContext _outOfTimeDbContext;
        private readonly IClusterService _clusterService;
        
        public ClassService(OutOfTimeDbContext outOfTimeDbContext, IClusterService clusterService)
        {
            _outOfTimeDbContext = outOfTimeDbContext;
            _clusterService = clusterService;
        }

        public async Task<ClassOperationResult> QueryClasses(ClassQueryDto classQueryDto)
        {
            var classes = _outOfTimeDbContext.Classes
                .Include(x => x.TimeSlot)
                .Include(x => x.Cluster)
                .Include(x => x.Educator)
                .Include(x => x.LectureHall)
                .Include(x => x.Type)
                .OrderByDescending(x => DateOnly.FromDateTime(x.Date))
                .Take(42);

            if (classQueryDto.StartDate is not null)
            {
                DateTime startDate = classQueryDto.StartDate ?? throw new ArgumentNullException();
                classes = classes.Where(x => DateOnly.FromDateTime(x.Date) >= DateOnly.FromDateTime(startDate));
            }

            if (classQueryDto.EndDate is not null)
            {
                DateTime endDate = classQueryDto.EndDate ?? throw new ArgumentNullException();
                classes = classes.Where(x => DateOnly.FromDateTime(x.Date) <= DateOnly.FromDateTime(endDate));
            }

            if (classQueryDto.ClusterNumber is not null)
            {
                var cluster = await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).SingleOrDefaultAsync(x => x.Number == classQueryDto.ClusterNumber);
                if (cluster is null)
                {
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.ClusterNotFound, classQueryDto.ClusterNumber);
                }
                var associatedClusters = await _clusterService.GetAssociatedClusters(cluster);

                classes = classes.Where(x => associatedClusters.Any(clust => clust == x.Cluster));
            }

            if (classQueryDto.EducatorId is not null)
            {
                var educator = await _outOfTimeDbContext.Educators.SingleOrDefaultAsync(x => x.Id == classQueryDto.EducatorId);
                if (educator is null)
                {
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.EducatorNotFound, classQueryDto.EducatorId.ToString());
                }

                classes = classes.Where(x => x.Educator == educator);
            }

            if (classQueryDto.LectureHallId is not null)
            {
                var lectureHall = await _outOfTimeDbContext.LectureHalls.SingleOrDefaultAsync(x => x.Id == classQueryDto.LectureHallId);
                if (lectureHall is null)
                {
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.LectureHallNotFound, classQueryDto.LectureHallId.ToString());
                }

                classes = classes.Where(x => x.LectureHall == lectureHall);
            }

            var result = await classes.ToListAsync();

            return GenerateDefaultClassOperationResult(ClassOperationStatus.Success, queryResult: result);
        }

        public async Task<ClassOperationResult> TryCreateClass(ClassDto classDto)
        {
            TimeSlot? timeSlot = await GetTimeSlotIfExists(classDto.TimeSlotNumber);
            if (timeSlot is null)
                return GenerateDefaultClassOperationResult(ClassOperationStatus.TimeSlotNotFound, classDto.TimeSlotNumber.ToString());

            Cluster? cluster = await GetClusterIfExists(classDto.ClusterNumber);
            if (cluster is null)
                return GenerateDefaultClassOperationResult(ClassOperationStatus.ClusterNotFound, classDto.ClusterNumber);

            ClassType? classType = null;
            if (classDto.ClassTypeName is not null)
            {
                classType = await GetClassTypeIfExists(classDto.ClassTypeName);
                if (classType is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.ClassTypeNotFound, classDto.ClassTypeName);
            }

            Educator? educator = null;
            if (classDto.EducatorId is not null)
            {
                educator = await GetEducatorIfExists(classDto.EducatorId);
                if (educator is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.EducatorNotFound, classDto.EducatorId.ToString());
            }

            LectureHall? lectureHall = null;
            if (classDto.LectureHallId is not null)
            {
                lectureHall = await GetLectureHallIfExists(classDto.LectureHallId);
                if (lectureHall is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.LectureHallNotFound, classDto.LectureHallId.ToString());
            }

            var newClass = new Class
            {
                Date = classDto.Date,
                TimeSlot = timeSlot,
                Cluster = cluster,
                Educator = educator,
                LectureHall = lectureHall,
                Type = classType
            };

            var checkResult = await CheckConflictPresent(newClass);
            if (checkResult.Status is not ClassOperationStatus.Success)
            {
                return checkResult;
            }

            await _outOfTimeDbContext.Classes.AddAsync(newClass);
            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultClassOperationResult(status: ClassOperationStatus.ClassEdited);
        }


        public async Task<ClassOperationResult> TryEditClass(Guid id, ClassEditDto classEditDto, bool nullMode = false)
        {
            Class? @class = await _outOfTimeDbContext.Classes
                .Include(x => x.TimeSlot)
                .Include(x => x.Cluster)
                .Include(x => x.Educator)
                .Include(x => x.LectureHall)
                .Include(x => x.Type)
                .SingleOrDefaultAsync(x=> x.Id == id);

            if (@class is null)
            {
                return GenerateDefaultClassOperationResult(status: ClassOperationStatus.ClassNotFound, id.ToString());
            }

            if (classEditDto.Date is null && nullMode)
            {
                return GenerateDefaultClassOperationResult(ClassOperationStatus.UnspecifiedDate);
            }

            TimeSlot? timeSlot = null;
            if (classEditDto.TimeSlotNumber is not null)
            {
                timeSlot = await GetTimeSlotIfExists(classEditDto.TimeSlotNumber);
                if (timeSlot is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.TimeSlotNotFound, classEditDto.TimeSlotNumber.ToString());
            }
            else if (nullMode)
            {
                return GenerateDefaultClassOperationResult(ClassOperationStatus.UnspecifiedTimeSlot);
            }

            Cluster? cluster = null;
            if (classEditDto.ClusterNumber is not null)
            {
                cluster = await GetClusterIfExists(classEditDto.ClusterNumber);
                if (cluster is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.ClusterNotFound, classEditDto.ClusterNumber);
            }
            else if (nullMode)
            {
                return GenerateDefaultClassOperationResult(ClassOperationStatus.UnspecifiedCluster);
            }

            ClassType? classType = null;
            if (classEditDto.ClassTypeName is not null)
            {
                classType = await GetClassTypeIfExists(classEditDto.ClassTypeName);
                if (classType is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.ClassTypeNotFound, classEditDto.ClassTypeName);
            }

            Educator? educator = null;
            if (classEditDto.EducatorId is not null)
            {
                educator = await GetEducatorIfExists(classEditDto.EducatorId);
                if (educator is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.EducatorNotFound, classEditDto.EducatorId.ToString());
            }

            LectureHall? lectureHall = null;
            if (classEditDto.LectureHallId is not null)
            {
                lectureHall = await GetLectureHallIfExists(classEditDto.LectureHallId);
                if (lectureHall is null)
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.LectureHallNotFound, classEditDto.LectureHallId.ToString());
            }

            
            if (nullMode)
            {
                @class.Date = classEditDto.Date ?? throw new ArgumentNullException();
                @class.TimeSlot = timeSlot ?? throw new ArgumentNullException();
                @class.Cluster = cluster ?? throw new ArgumentNullException();
                @class.Educator = educator;
                @class.LectureHall = lectureHall;
                @class.Type = classType;
            }
            else
            {
                @class.Date = classEditDto.Date ?? @class.Date;
                @class.TimeSlot = timeSlot ?? @class.TimeSlot;
                @class.Cluster = cluster ?? @class.Cluster;
                @class.Educator = educator ?? @class.Educator;
                @class.LectureHall = lectureHall ?? @class.LectureHall;
                @class.Type = classType ?? @class.Type;
            }

            var checkResult = await CheckConflictPresent(@class, id);
            if (checkResult.Status is not ClassOperationStatus.Success)
            {
                return checkResult;
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultClassOperationResult(status: ClassOperationStatus.ClassEdited);
        }

        private async Task<ClassOperationResult> CheckConflictPresent(Class newClass, Guid? thisClassId = null)
        {
            if (newClass is null) throw new ArgumentNullException(nameof(newClass));

            // selects classes that share date and time slot with that being created
            var concurrentClasses = _outOfTimeDbContext.Classes.Where(x => x.TimeSlot.Number == newClass.TimeSlot.Number
            && DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(newClass.Date));

            if (thisClassId is not null)
            {
                concurrentClasses = concurrentClasses.Where(x => x.Id != thisClassId);
            }

            // checks whether the cluster is busy at the specified time
            var associatedClusters = await _clusterService.GetAssociatedClusters(newClass.Cluster);
            foreach (var clust in associatedClusters)
            {
                if (await concurrentClasses.AnyAsync(x => x.Cluster.Number == clust.Number))
                {
                    return GenerateDefaultClassOperationResult(ClassOperationStatus.ClusterOccupied, clust.Number);
                }
            }

            // checks whether the educator is busy at the specified time
            if ((newClass.Educator is not null) ? await concurrentClasses.Where(x => x.Educator != null).AnyAsync(x => x.Educator!.Id == newClass.Educator.Id) : false)
            {
                return GenerateDefaultClassOperationResult(ClassOperationStatus.EducatorOccupied, newClass.Educator.Id.ToString());
            }

            // checks whether the lecture hall is occupied at the specified time
            if ((newClass.LectureHall is not null) ? await concurrentClasses.Where(x => x.LectureHall != null).AnyAsync(x => x.LectureHall!.Id == newClass.LectureHall.Id) : false)
            {
                return GenerateDefaultClassOperationResult(ClassOperationStatus.LectureHallOccupied, newClass.LectureHall.Id.ToString());
            }

            return GenerateDefaultClassOperationResult(ClassOperationStatus.Success);
        }

        private async Task<Cluster?> GetClusterIfExists(string? number)
        {
            if (number == null) return null;
            var cluster = await _outOfTimeDbContext.Clusters.Include(x => x.SuperCluster).SingleOrDefaultAsync(x => x.Number == number);
            return cluster;
        }
        private async Task<TimeSlot?> GetTimeSlotIfExists(int? number)
        {
            if (number == null) return null;
            var timeSlot = await _outOfTimeDbContext.TimeSlots.SingleOrDefaultAsync(x => x.Number == number);
            return timeSlot;
        }
        private async Task<Educator?> GetEducatorIfExists(Guid? id)
        {
            if (id == null) return null;
            var educator = await _outOfTimeDbContext.Educators.SingleOrDefaultAsync(x => x.Id == id);
            return educator;
        }
        private async Task<LectureHall?> GetLectureHallIfExists(Guid? id)
        {
            if (id == null) return null;
            var lectureHall = await _outOfTimeDbContext.LectureHalls.SingleOrDefaultAsync(x => x.Id == id);
            return lectureHall;
        }
        private async Task<ClassType?> GetClassTypeIfExists(string? name)
        {
            if (name == null) return null;
            var classType = await _outOfTimeDbContext.ClassTypes.SingleOrDefaultAsync(x => x.Name == name);
            return classType;
        }
    }
}
