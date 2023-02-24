using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services;

public interface ICampusBuildingService
{
    public Task<List<CampusBuilding>> GetAll();
    public Task<Result> Create(CampusBuildingDto campusBuildingDto);
    public Task<Result> Edit(Guid id, CampusBuildingDto campusBuildingDto);
    public Task<Result> Delete(Guid id);
}