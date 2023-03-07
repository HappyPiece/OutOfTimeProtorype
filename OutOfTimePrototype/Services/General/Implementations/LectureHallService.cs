using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.General.Implementations;

public class LectureHallService : ILectureHallService
{
    private readonly IMapper _mapper;
    private readonly OutOfTimeDbContext _outOfTimeDbContext;

    public LectureHallService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    public async Task<List<LectureHall>> GetAll()
    {
        return await _outOfTimeDbContext.LectureHalls.Include(x => x.HostBuilding).ToListAsync();
    }

    public async Task<List<LectureHall>> GetAllUnoccupied(int timeSlotNumber, DateTime date)
    {
        var occupiedLectureHalls = await _outOfTimeDbContext.Classes.Where(@class =>
                @class.TimeSlot.Number == timeSlotNumber &&
                DateOnly.FromDateTime(@class.Date) == DateOnly.FromDateTime(date))
            .Select(@class => @class.LectureHall)
            .ToListAsync();

        return await _outOfTimeDbContext.LectureHalls.Where(hall => !occupiedLectureHalls.Contains(hall))
            .Include(x => x.HostBuilding).ToListAsync();
    }

    // TODO: maybe need to return LectureHallDto
    public async Task<List<LectureHall>> GetByBuilding(Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.Where(hall => hall.HostBuilding.Id == hostBuildingId)
            .Include(x => x.HostBuilding).ToListAsync();
    }

    public async Task<Result> CreateLectureHall(LectureHallCreateDto hallDto)
    {
        var host = await _outOfTimeDbContext.CampusBuildings.FirstOrDefaultAsync(x => x.Id == hallDto.HostBuildingId);
        if (host is null)
        {
            var e = new RecordNotFoundException(
                $"Building with Id '{hallDto.HostBuildingId}' does not exist");
            return Result.Fail(e);
        }

        if (await SameHallExists(hallDto.Name, hallDto.HostBuildingId))
        {
            var e = new AlreadyExistsException(
                $"Lecture hall with name: '{hallDto.Name}' already exists in building '{hallDto.HostBuildingId}'");
            return Result.Fail(e);
        }

        var entity = new LectureHall
        {
            HostBuildingId = hallDto.HostBuildingId,
            Name = hallDto.Name,
            Capacity = hallDto.Capacity,
            HostBuilding = host
        };
        _outOfTimeDbContext.LectureHalls.Add(entity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> EditLectureHall(Guid id, LectureHallUpdateDto hallUpdateDto)
    {
        var dbEntity = await _outOfTimeDbContext.LectureHalls.FindAsync(id);

        if (dbEntity is null)
            return new RecordNotFoundException($"Lecture hall with id: '{id}' does not exists");

        CampusBuilding? newHost = null;
        if (hallUpdateDto.HostBuildingId != null)
        {
            newHost = await _outOfTimeDbContext.CampusBuildings.FirstOrDefaultAsync(x =>
                x.Id == hallUpdateDto.HostBuildingId);
            if (newHost is null)
                return new RecordNotFoundException(
                    $"Campus building with id: '{hallUpdateDto.HostBuildingId}' does not exists");
            if (await SameHallExists(hallUpdateDto.Name ?? dbEntity.Name,
                    hallUpdateDto.HostBuildingId ?? throw new ArgumentNullException()))
            {
                var e = new AlreadyExistsException(
                    $"Lecture hall with name: '{hallUpdateDto.Name}' already exists in building '{hallUpdateDto.HostBuildingId}'");
                return Result.Fail(e);
            }
        }

        {
            dbEntity.Capacity = hallUpdateDto.Capacity ?? dbEntity.Capacity;
            dbEntity.Name = hallUpdateDto.Name ?? dbEntity.Name;
            dbEntity.HostBuildingId = hallUpdateDto.HostBuildingId ?? dbEntity.HostBuildingId;
            dbEntity.HostBuilding = newHost ?? dbEntity.HostBuilding;
        }

        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<bool> SameHallExists(string name, Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.AnyAsync(hall =>
            hall.Name == name && hall.HostBuilding.Id == hostBuildingId);
    }
}