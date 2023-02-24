using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.Configurations;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Services.Interfaces;
using System.Diagnostics.Metrics;
using OutOfTimePrototype.Utilities;
using static OutOfTimePrototype.Utilities.ClassUtilities;
using static OutOfTimePrototype.Utilities.ClassUtilities.ClassOperationResult;
using LanguageExt.ClassInstances;
using OutOfTimePrototype.Dal.Models;

namespace OutOfTimePrototype.Services.Implementations
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
                .Include(x => x.Subject)
                .AsQueryable();

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
                var cluster = await GetClusterIfExists(classQueryDto.ClusterNumber);
                if (cluster is null)
                {
                    return GenerateDefaultOperationResult(OperationStatus.ClusterNotFound, classQueryDto.ClusterNumber);
                }
                var associatedClusters = await _clusterService.GetAssociatedClusters(cluster);

                classes = classes.Where(x => associatedClusters.Any(clust => clust == x.Cluster));
            }

            if (classQueryDto.EducatorId is not null)
            {
                var educator = await GetEducatorIfExists(classQueryDto.EducatorId);
                if (educator is null)
                {
                    return GenerateDefaultOperationResult(OperationStatus.EducatorNotFound, classQueryDto.EducatorId.ToString());
                }

                classes = classes.Where(x => x.Educator == educator);
            }

            if (classQueryDto.SubjectId is not null)
            {
                var subject = await GetSubjectIfExists(classQueryDto.LectureHallId);
                if (subject is null)
                {
                    return GenerateDefaultOperationResult(OperationStatus.LectureHallNotFound, classQueryDto.SubjectId.ToString());
                }

                classes = classes.Where(x => x.Subject == subject);
            }

            if (classQueryDto.LectureHallId is not null)
            {
                var lectureHall = await GetLectureHallIfExists(classQueryDto.LectureHallId);
                if (lectureHall is null)
                {
                    return GenerateDefaultOperationResult(OperationStatus.LectureHallNotFound, classQueryDto.LectureHallId.ToString());
                }

                classes = classes.Where(x => x.LectureHall == lectureHall);
            }

            var cardinality = await classes.OrderByDescending(x => DateOnly.FromDateTime(x.Date)).CountAsync();
            if (cardinality > Constants.MaxQuerySize)
            {
                return GenerateDefaultOperationResult(OperationStatus.QueryTooLarge);
            }
            classes = classes.OrderByDescending(x => DateOnly.FromDateTime(x.Date)).Take(Constants.MaxQuerySize);

            var result = await classes.ToListAsync();

            return GenerateDefaultOperationResult(OperationStatus.Success, queryResult: result);
        }

        public async Task<ClassOperationResult> TryDeleteClass(Guid id)
        {
            return await TryDeleteClass(id, false);
        }

        private async Task<ClassOperationResult> TryDeleteClass(Guid id, bool isTransactionUnit = false)
        {
            Class? @class = await _outOfTimeDbContext.Classes.SingleOrDefaultAsync(x => x.Id == id);

            if (@class is null)
            {
                return GenerateDefaultOperationResult(OperationStatus.ClassNotFound, id.ToString());
            }

            return await TryDeleteClass(@class);
        }

        private async Task<ClassOperationResult> TryDeleteClass(Class @class, bool isTransactionUnit = false)
        {
            _outOfTimeDbContext.Classes.Remove(@class);

            if (!isTransactionUnit)
            {
                await _outOfTimeDbContext.SaveChangesAsync();
            }

            return GenerateDefaultOperationResult(OperationStatus.ClassDeleted, @class.Id.ToString());
        }

        public async Task<ClassOperationResult> TryDeleteClasses(ClassQueryDto classQueryDto)
        {
            var queryResult = await QueryClasses(classQueryDto);
            if (queryResult.Status is not OperationStatus.Success)
            {
                return queryResult;
            }

            foreach (var @class in queryResult.QueryResult ?? throw new ArgumentNullException())
            {
                var interimResult = await TryDeleteClass(@class, isTransactionUnit: true);
                if (interimResult.Status is not OperationStatus.ClassDeleted)
                {
                    return interimResult;
                }
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(OperationStatus.ClassesDeleted, queryResult.QueryResult.Count.ToString());
        }

        public async Task<ClassOperationResult> TryCreateClass(ClassDto classDto)
        {
            classDto.Format();
            return await TryCreateClass(classDto, false);
        }

        private async Task<ClassOperationResult> TryCreateClass(ClassDto classDto, bool isTransactionUnit = false)
        {
            var initializationResult = await TryInitializeClass(classDto);
            if (initializationResult.Status is not OperationStatus.Success)
            {
                return initializationResult;
            }
            var newClass = initializationResult.OperationResult;

            var checkResult = await CheckConflictPresent(newClass ?? throw new ArgumentNullException("Expected class to be not null here"));
            if (checkResult.Status is not OperationStatus.Success)
            {
                return checkResult;
            }

            await _outOfTimeDbContext.Classes.AddAsync(newClass);

            if (!isTransactionUnit)
            {
                await _outOfTimeDbContext.SaveChangesAsync();
            }

            return GenerateDefaultOperationResult(status: OperationStatus.ClassCreated, arg: newClass.Id.ToString());
        }

        public async Task<ClassOperationResult> TryCreateClasses(ClassQueryDto classQueryDto, ClassDto classDto)
        {
            classDto.Format();

            if (!classQueryDto.ValidateAsForCreateClasses().IsValid)
            {
                return GenerateDefaultOperationResult(OperationStatus.BadQuery, modelState: classQueryDto.ValidateAsForCreateClasses());
            }

            var dates = classQueryDto.DayOfWeek.GetDayOfWeekFromBetweenDates(
                classQueryDto.StartDate ?? throw new ArgumentNullException(), 
                classQueryDto.EndDate ?? throw new ArgumentNullException());

            var @class = classDto;

            foreach (var date in dates)
            {
                @class.Date = date;
                var interimResult = await TryCreateClass(@class, isTransactionUnit: true);
                if (interimResult.Status is not OperationStatus.ClassCreated)
                {
                    return interimResult;
                }
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(status: OperationStatus.ClassesCreated, arg: dates.Count.ToString());
        }

        public async Task<ClassOperationResult> TryEditClass(Guid id, ClassEditDto classEditDto, bool nullMode = false)
        {
            classEditDto.Format();
            return await TryEditClass(id, classEditDto, nullMode, false);
        }

        private async Task<ClassOperationResult> TryEditClass(Class @class, ClassEditDto classEditDto, bool nullMode = false, bool isTransactionUnit = false)
        {
            if (classEditDto.Date is null && nullMode)
            {
                return GenerateDefaultOperationResult(OperationStatus.UnspecifiedDate);
            }

            TimeSlot? timeSlot = null;
            if (classEditDto.TimeSlotNumber is not null)
            {
                timeSlot = await GetTimeSlotIfExists(classEditDto.TimeSlotNumber);
                if (timeSlot is null)
                    return GenerateDefaultOperationResult(OperationStatus.TimeSlotNotFound, classEditDto.TimeSlotNumber.ToString());
            }
            else if (nullMode)
            {
                return GenerateDefaultOperationResult(OperationStatus.UnspecifiedTimeSlot);
            }

            Cluster? cluster = null;
            if (classEditDto.ClusterNumber is not null)
            {
                cluster = await GetClusterIfExists(classEditDto.ClusterNumber);
                if (cluster is null)
                    return GenerateDefaultOperationResult(OperationStatus.ClusterNotFound, classEditDto.ClusterNumber);
            }
            else if (nullMode)
            {
                return GenerateDefaultOperationResult(OperationStatus.UnspecifiedCluster);
            }

            ClassType? classType = null;
            if (classEditDto.ClassTypeName is not null)
            {
                classType = await GetClassTypeIfExists(classEditDto.ClassTypeName);
                if (classType is null)
                    return GenerateDefaultOperationResult(OperationStatus.ClassTypeNotFound, classEditDto.ClassTypeName);
            }

            Educator? educator = null;
            if (classEditDto.EducatorId is not null)
            {
                educator = await GetEducatorIfExists(classEditDto.EducatorId);
                if (educator is null)
                    return GenerateDefaultOperationResult(OperationStatus.EducatorNotFound, classEditDto.EducatorId.ToString());
            }

            Subject? subject = null;
            if (classEditDto.SubjectId is not null)
            {
                subject = await GetSubjectIfExists(classEditDto.SubjectId);
                if (subject is null)
                    return GenerateDefaultOperationResult(OperationStatus.SubjectNotFound, classEditDto.SubjectId.ToString());
            }

            LectureHall? lectureHall = null;
            if (classEditDto.LectureHallId is not null)
            {
                lectureHall = await GetLectureHallIfExists(classEditDto.LectureHallId);
                if (lectureHall is null)
                    return GenerateDefaultOperationResult(OperationStatus.LectureHallNotFound, classEditDto.LectureHallId.ToString());
            }


            if (nullMode)
            {
                @class.Date = classEditDto.Date ?? throw new ArgumentNullException();
                @class.TimeSlot = timeSlot ?? throw new ArgumentNullException();
                @class.Cluster = cluster ?? throw new ArgumentNullException();
                @class.Educator = educator;
                @class.Subject = subject;
                @class.LectureHall = lectureHall;
                @class.Type = classType;
            }
            else
            {
                @class.Date = classEditDto.Date ?? @class.Date;
                @class.TimeSlot = timeSlot ?? @class.TimeSlot;
                @class.Cluster = cluster ?? @class.Cluster;
                @class.Educator = educator ?? @class.Educator;
                @class.Subject = subject ?? @class.Subject;
                @class.LectureHall = lectureHall ?? @class.LectureHall;
                @class.Type = classType ?? @class.Type;
            }

            var checkResult = await CheckConflictPresent(@class);
            if (checkResult.Status is not OperationStatus.Success)
            {
                return checkResult;
            }

            if (!isTransactionUnit)
            {
                await _outOfTimeDbContext.SaveChangesAsync();
            }

            return GenerateDefaultOperationResult(status: OperationStatus.ClassEdited, arg: @class.Id.ToString());
        }

        private async Task<ClassOperationResult> TryEditClass(Guid id, ClassEditDto classEditDto, bool nullMode = false, bool isTransactionUnit = false)
        {
            Class? @class = await _outOfTimeDbContext.Classes
                .Include(x => x.TimeSlot)
                .Include(x => x.Cluster)
                .Include(x => x.Educator)
                .Include(x => x.LectureHall)
                .Include(x => x.Type)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (@class is null)
            {
                return GenerateDefaultOperationResult(status: OperationStatus.ClassNotFound, id.ToString());
            }

            return await TryEditClass(@class, classEditDto, nullMode, isTransactionUnit);
        }

        public async Task<ClassOperationResult> TryEditClasses(ClassQueryDto classQueryDto, ClassEditDto classEditDto, bool nullMode)
        {
            classEditDto.Format();
            var queryResult = await QueryClasses(classQueryDto);
            if (queryResult.Status is not OperationStatus.Success)
            {
                return queryResult;
            }

            foreach (var @class in queryResult.QueryResult ?? throw new ArgumentNullException())
            {
                var interimResult = await TryEditClass(@class, classEditDto, nullMode, isTransactionUnit: true);
                if (interimResult.Status is not OperationStatus.ClassEdited)
                { 
                    return interimResult;
                }
            }

            await _outOfTimeDbContext.SaveChangesAsync();

            return GenerateDefaultOperationResult(OperationStatus.ClassesEdited, queryResult.QueryResult.Count.ToString());
        }

        private async Task<ClassOperationResult> TryInitializeClass(ClassDto classDto)
        {
            TimeSlot? timeSlot = await GetTimeSlotIfExists(classDto.TimeSlotNumber);
            if (timeSlot is null)
                return GenerateDefaultOperationResult(OperationStatus.TimeSlotNotFound, classDto.TimeSlotNumber.ToString());

            Cluster? cluster = await GetClusterIfExists(classDto.ClusterNumber);
            if (cluster is null)
                return GenerateDefaultOperationResult(OperationStatus.ClusterNotFound, classDto.ClusterNumber);

            ClassType? classType = null;
            if (classDto.ClassTypeName is not null)
            {
                classType = await GetClassTypeIfExists(classDto.ClassTypeName);
                if (classType is null)
                    return GenerateDefaultOperationResult(OperationStatus.ClassTypeNotFound, classDto.ClassTypeName);
            }

            Educator? educator = null;
            if (classDto.EducatorId is not null)
            {
                educator = await GetEducatorIfExists(classDto.EducatorId);
                if (educator is null)
                    return GenerateDefaultOperationResult(OperationStatus.EducatorNotFound, classDto.EducatorId.ToString());
            }

            LectureHall? lectureHall = null;
            if (classDto.LectureHallId is not null)
            {
                lectureHall = await GetLectureHallIfExists(classDto.LectureHallId);
                if (lectureHall is null)
                    return GenerateDefaultOperationResult(OperationStatus.LectureHallNotFound, classDto.LectureHallId.ToString());
            }

            Subject? subject = null;
            if (classDto.SubjectId is not null)
            {
                subject = await GetSubjectIfExists(classDto.SubjectId);
                if (subject is null)
                    return GenerateDefaultOperationResult(OperationStatus.SubjectNotFound, classDto.SubjectId.ToString());
            }

            var newClass = new Class
            {
                Date = classDto.Date,
                TimeSlot = timeSlot,
                Cluster = cluster,
                Educator = educator,
                Subject = subject,
                LectureHall = lectureHall,
                Type = classType
            };

            return GenerateDefaultOperationResult(OperationStatus.Success, operationResult: newClass);
        }

        private async Task<ClassOperationResult> CheckConflictPresent(Class newClass)
        {
            if (newClass is null) throw new ArgumentNullException(nameof(newClass));

            // selects classes that share date and time slot with that being created
            var concurrentClasses = _outOfTimeDbContext.Classes.Where(x => x.TimeSlot.Number == newClass.TimeSlot.Number
            && DateOnly.FromDateTime(x.Date) == DateOnly.FromDateTime(newClass.Date));

            concurrentClasses = concurrentClasses.Where(x => x.Id != newClass.Id);

            // checks whether the cluster is busy at the specified time
            var associatedClusters = await _clusterService.GetAssociatedClusters(newClass.Cluster);
            foreach (var clust in associatedClusters)
            {
                if (await concurrentClasses.AnyAsync(x => x.Cluster.Number == clust.Number))
                {
                    return GenerateDefaultOperationResult(OperationStatus.ClusterOccupied, clust.Number);
                }
            }

            // checks whether the educator is busy at the specified time
            if (newClass.Educator is not null ? await concurrentClasses.Where(x => x.Educator != null).AnyAsync(x => x.Educator!.Id == newClass.Educator.Id) : false)
            {
                return GenerateDefaultOperationResult(OperationStatus.EducatorOccupied, newClass.Educator.Id.ToString());
            }

            // checks whether the lecture hall is occupied at the specified time
            if (newClass.LectureHall is not null ? await concurrentClasses.Where(x => x.LectureHall != null).AnyAsync(x => x.LectureHall!.Id == newClass.LectureHall.Id) : false)
            {
                return GenerateDefaultOperationResult(OperationStatus.LectureHallOccupied, newClass.LectureHall.Id.ToString());
            }

            return GenerateDefaultOperationResult(OperationStatus.Success);
        }

        private async Task<Cluster?> GetClusterIfExists(string? number)
        {
            if (number == null) return null;
            var cluster = await _clusterService.TryGetCluster(number);
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

        private async Task<Subject?> GetSubjectIfExists(Guid? id)
        {
            if (id == null) return null;
            var subject = await _outOfTimeDbContext.Subjects.SingleOrDefaultAsync(x => x.Id == id);
            return subject;
        }
        private async Task<ClassType?> GetClassTypeIfExists(string? name)
        {
            if (name == null) return null;
            var classType = await _outOfTimeDbContext.ClassTypes.SingleOrDefaultAsync(x => x.Name == name);
            return classType;
        }
    }
}
