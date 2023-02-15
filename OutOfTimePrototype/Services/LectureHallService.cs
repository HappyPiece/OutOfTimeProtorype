using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;

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

    private async Task<bool> IsLectureHallExists(string name, Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.AnyAsync(hall =>
            hall.Name == name && hall.HostBuilding.Id == hostBuildingId);
    }

    public async Task<List<LectureHall>> GetFreeLectureHalls(int timeSlotNumber, DateTime date)
    {
        var occupiedLectureHalls = await _outOfTimeDbContext.Classes.Where(@class =>
                @class.TimeSlot.Number == timeSlotNumber &&
                DateOnly.FromDateTime(@class.Date) == DateOnly.FromDateTime(date))
            .Select(@class => @class.LectureHall)
            .ToListAsync();

        return await _outOfTimeDbContext.LectureHalls.Where(hall => !occupiedLectureHalls.Contains(hall)).ToListAsync();
    }

    public async Task<List<LectureHall>> GetLectureHallsByBuilding(Guid hostBuildingId)
    {
        return await _outOfTimeDbContext.LectureHalls.Where(hall => hall.HostBuilding.Id == hostBuildingId)
            .ToListAsync();
    }

    public async Task CreateLectureHall(LectureHallDTO hallDto)
    {
        if (await IsLectureHallExists(hallDto.Name, hallDto.HostBuildingId))
        {
            throw new AlreadyExistsException(
                $"Lecture hall with name: '{hallDto.Name}' and building id: '{hallDto.HostBuildingId}' already exists");
        }

        var entity = _mapper.Map<LectureHall>(hallDto);
        _outOfTimeDbContext.LectureHalls.Add(entity);
        await _outOfTimeDbContext.SaveChangesAsync();
    }
    
    public async Task EditLectureHall(Guid id, LectureHallUpdateModel hallUpdateModel)
    {
        var dbEntity = await _outOfTimeDbContext.LectureHalls.FindAsync(id);

        if (dbEntity is null)
        {
            throw new RecordNotFoundException($"Lecture hall with id: '{id}' do not exists");
        }

        var updatedEntity = _mapper.Map(hallUpdateModel, dbEntity);
        _outOfTimeDbContext.LectureHalls.Add(updatedEntity);
        await _outOfTimeDbContext.SaveChangesAsync();
    }
}