using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services;

public interface ILectureHallService
{
    Task<List<LectureHall>> GetAllUnoccupied(int timeSlotNumber, DateTime date);
    Task<List<LectureHall>> GetByBuilding(Guid hostBuildingId);
    Task<Result> CreateLectureHall(LectureHallDto hallDto);
    Task<Result> EditLectureHall(Guid id, LectureHallUpdateModel hallUpdateModel);
}