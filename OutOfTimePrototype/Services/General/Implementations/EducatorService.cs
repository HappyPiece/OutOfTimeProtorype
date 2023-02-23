using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.Interfaces;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.Implementations;

public class EducatorService : IEducatorService
{
    private readonly IMapper _mapper;
    private readonly OutOfTimeDbContext _outOfTimeDbContext;

    public EducatorService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
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

        return await _outOfTimeDbContext.Educators.Where(educator => !occupiedEducators.Contains(educator))
            .ToListAsync();
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

    public async Task<Result> Delete(Guid id)
    {
        if (await _outOfTimeDbContext.Educators.AnyAsync(e => e.Id == id))
            return Result.Fail(new RecordNotFoundException($"Educator with id '{id}' not found"));

        var entityToRemove = new Educator { Id = id };
        _outOfTimeDbContext.Educators.Remove(entityToRemove);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<bool> IsExistsById(Guid id)
    {
        return await _outOfTimeDbContext.Educators.AnyAsync(educator => educator.Id == id);
    }
}