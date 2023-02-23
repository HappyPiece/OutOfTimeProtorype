using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.Interfaces;

public interface IEducatorService
{
    Task<List<Educator>> GetAll();
    Task<List<Educator>> GetAllUnoccupied(int timeSlotNumber, DateTime dateTime);
    Task Create(EducatorDto educatorDto);
    Task<Result> Edit(Guid id, EducatorDto educatorDto);
    Task<Result> Delete(Guid id);
}