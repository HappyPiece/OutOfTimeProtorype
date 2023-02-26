using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OutOfTimePrototype.DAL;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Exceptions;
using OutOfTimePrototype.Services.General.Interfaces;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.General.Implementations;

public class CampusBuildingService : ICampusBuildingService
{
    private readonly IMapper _mapper;
    private readonly OutOfTimeDbContext _outOfTimeDbContext;

    public CampusBuildingService(OutOfTimeDbContext outOfTimeDbContext, IMapper mapper)
    {
        _outOfTimeDbContext = outOfTimeDbContext;
        _mapper = mapper;
    }

    public async Task<List<CampusBuilding>> GetAll()
    {
        return await _outOfTimeDbContext.CampusBuildings.ToListAsync();
    }

    public async Task<Result> Create(CampusBuildingDto campusBuildingDto)
    {
        if (string.IsNullOrEmpty(campusBuildingDto.Address))
            return new ValidationException("Address of campus building must be present");

        if (await IsExistsByAddress(campusBuildingDto.Address))
            return new AlreadyExistsException(
                $"Campus building with {campusBuildingDto.Address} address already exists");
        if (campusBuildingDto.LectureHalls != null &&
            !await _outOfTimeDbContext.LectureHalls.AllAsync(hall => campusBuildingDto.LectureHalls.Contains(hall.Id)))
            return new RecordNotFoundException("Not all present lecture halls actually exists");

        var entity = _mapper.Map<CampusBuilding>(campusBuildingDto);
        _outOfTimeDbContext.CampusBuildings.Add(entity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> Edit(Guid id, CampusBuildingDto campusBuildingDto)
    {
        var dbEntity = await _outOfTimeDbContext.CampusBuildings.FindAsync(id);

        if (dbEntity is null)
            return Result.Fail(new RecordNotFoundException($"Campus building with id '{id}' not found"));

        if (!string.IsNullOrEmpty(campusBuildingDto.Address) &&
            await _outOfTimeDbContext.CampusBuildings.AnyAsync(b => b.Address == campusBuildingDto.Address))
            return Result.Fail(
                new AlreadyExistsException(
                    $"Campus building with address '{campusBuildingDto.Address}' already exists"));

        var updatedEntity = _mapper.Map(campusBuildingDto, dbEntity);
        _outOfTimeDbContext.CampusBuildings.Update(updatedEntity);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> Delete(Guid id)
    {
        if (await _outOfTimeDbContext.CampusBuildings.AnyAsync(b => b.Id == id))
            return Result.Fail(new RecordNotFoundException($"Campus building with id '{id}' not found"));

        var entityToRemove = new Educator { Id = id };
        _outOfTimeDbContext.Educators.Remove(entityToRemove);
        await _outOfTimeDbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<bool> IsExistsByAddress(string address)
    {
        return await _outOfTimeDbContext.CampusBuildings.AnyAsync(building => building.Address == address);
    }
}