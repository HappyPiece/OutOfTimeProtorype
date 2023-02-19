using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.Implementations;

public class LectureHallService : ILectureHallService
{
    private readonly IMapper _mapper;
    private readonly OutOfTimeDbContext _outOfTimeDbContext;

    public LectureHallService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    public async Task<List<LectureHall>> GetAllUnoccupied(int timeSlotNumber, DateTime date)
    {
        var occupiedLectureHalls = await _outOfTimeDbContext.Classes.Where(@class =>
                @class.TimeSlot.Number == timeSlotNumber &&
                DateOnly.FromDateTime(@class.Date) == DateOnly.FromDateTime(date))
            .Select(@class => @class.LectureHall)
            .ToListAsync();

        return await _outOfTimeDbContext.LectureHalls.Where(hall => !occupiedLectureHalls.Contains(hall)).ToListAsync();
    }

    // TODO: maybe need to return LectureHallDto
    public async Task<List<LectureHall>> GetByBuilding(Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.Where(hall => hall.HostBuilding.Id == hostBuildingId)
            .ToListAsync();
    }

    public async Task<Result> CreateLectureHall(LectureHallDto hallDto)
    {
        if (await IsLectureHallExists(hallDto.Name, hallDto.HostBuildingId))
        {
            var e = new AlreadyExistsException(
                $"Lecture hall with name: '{hallDto.Name}' and building id: '{hallDto.HostBuildingId}' already exists");
            return Result.Fail(e);
        }

        var entity = _mapper.Map<LectureHall>(hallDto);
        _outOfTimeDbContext.LectureHalls.Add(entity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> EditLectureHall(Guid id, LectureHallUpdateDto hallUpdateModel)
    {
        var dbEntity = await _outOfTimeDbContext.LectureHalls.FindAsync(id);

        if (dbEntity is null)
        {
            var e = new RecordNotFoundException($"Lecture hall with id: '{id}' do not exists");
            return Result.Fail(e);
        }

        var updatedEntity = _mapper.Map(hallUpdateModel, dbEntity);
        _outOfTimeDbContext.LectureHalls.Add(updatedEntity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<bool> IsLectureHallExists(string name, Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.AnyAsync(hall =>
            hall.Name == name && hall.HostBuilding.Id == hostBuildingId);
    }
}