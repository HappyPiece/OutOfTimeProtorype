using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.General.Interfaces;

public interface ICampusBuildingService
{
    public Task<List<CampusBuilding>> GetAll();
    public Task<Result> Create(CampusBuildingCreateDto campusBuildingDto);
    public Task<Result> Edit(Guid id, CampusBuildingEditDto campusBuildingDto);
    public Task<Result> Delete(Guid id);
}