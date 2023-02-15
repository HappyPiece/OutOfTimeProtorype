using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Services;

public class LectureHallService : ILectureHallService
{
    private readonly OutOfTimeDbContext _outOfTimeDbContext;
    private readonly IMapper _mapper;

    public LectureHallService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    private async Task<bool> IsLectureHallExists(Guid id)
    {
        return await _outOfTimeDbContext.LectureHalls.AnyAsync(hall => hall.Id == id);
    }

    private async Task<bool> IsLectureHallExists(string name, Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.AnyAsync(hall =>
            hall.Name == name && hall.HostBuilding.Id == hostBuildingId);
    }

    public async Task<List<LectureHall?>> GetFreeLectureHalls(int timeSlotNumber, DateTime date)
    {
        var occupiedLectureHalls = await _outOfTimeDbContext.Classes.Where(@class =>
                @class.TimeSlot.Number == timeSlotNumber &&
                DateOnly.FromDateTime(@class.Date) == DateOnly.FromDateTime(date))
            .Select(@class => @class.LectureHall)
            .ToListAsync();
        // WARNING: .Except() may be less performant than .Where()
        return await _outOfTimeDbContext.LectureHalls.Except(occupiedLectureHalls).ToListAsync();
    }

    public async Task<List<LectureHall>> GetLectureHallsByBuilding(Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.Where(hall => hall.HostBuilding.Id == hostBuildingId)
            .ToListAsync();
    }

    public async Task Create(LectureHallDTO hallDto)
    {
        if (await IsLectureHallExists(hallDto.Name, hallDto.HostBuildingId))
        {
            throw new NotImplementedException();
        }

        var entity = _mapper.Map<LectureHall>(hallDto);
        _outOfTimeDbContext.LectureHalls.Add(entity);
        await _outOfTimeDbContext.SaveChangesAsync();
    }

    public async Task Update(LectureHallUpdateModel hallUpdateModel)
    {
        var dbEntity = await _outOfTimeDbContext.LectureHalls.FindAsync(hallUpdateModel.Id);
        
        if (dbEntity is null)
        {
            throw new NotImplementedException();
        }

        var updatedEntity = _mapper.Map(hallUpdateModel, dbEntity);
        _outOfTimeDbContext.LectureHalls.Add(updatedEntity);
        await _outOfTimeDbContext.SaveChangesAsync();
    }
}

/**
 * select *
 * from LectureHall
 * except
 * select LectureHallId
 * from Class
 * where TimeSlotNumber = 1 and Date = 12.02.2023
 */