using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services;

public class EducatorService : IEducatorService
{
    private readonly OutOfTimeDbContext _outOfTimeDbContext;
    private readonly IMapper _mapper;

    public EducatorService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    private async Task<bool> IsExistsById(Guid id)
    {
        return await _outOfTimeDbContext.Educators.AnyAsync(educator => educator.Id == id);
    }
    
    public async Task<List<Educator>> GetAll()
    {
        return await _outOfTimeDbContext.Educators.ToListAsync();
    }

    public async Task<List<Educator>> GetAllUnoccupied(int timeSlotNumber, DateTime date)
    {
        var occupiedEducators = await _outOfTimeDbContext.Classes.Where(@class =>
                @class.TimeSlot.Number == timeSlotNumber &&
                DateOnly.FromDateTime(@class.Date) == DateOnly.FromDateTime(date))
            .Select(@class => @class.Educator)
            .ToListAsync();
        
        return await _outOfTimeDbContext.Educators.Where(educator => !occupiedEducators.Contains(educator)).ToListAsync();
    }

    public async Task Create(EducatorDto educatorDto)
    {
        var educator = _mapper.Map<Educator>(educatorDto);
        _outOfTimeDbContext.Educators.Add(educator);
        await _outOfTimeDbContext.SaveChangesAsync();
    }

    public async Task<Result> Edit(Guid id, EducatorDto educatorDto)
    {
        var dbEntity = await _outOfTimeDbContext.Educators.FindAsync(id);

        if (dbEntity is null)
        {
            var e = new RecordNotFoundException($"Educator with id '{id}' not found");
            return Result.Fail(e);
        }

        var updatedEntity = _mapper.Map(educatorDto, dbEntity);
        _outOfTimeDbContext.Educators.Update(updatedEntity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task Delete(Guid id)
    {
        var eToRemove = new Educator { Id = id };
        _outOfTimeDbContext.Educators.Remove(eToRemove);
        await _outOfTimeDbContext.SaveChangesAsync();
    }
}