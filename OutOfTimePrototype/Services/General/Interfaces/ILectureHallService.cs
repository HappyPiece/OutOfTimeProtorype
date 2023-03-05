using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;
using OutOfTimePrototype.Utilities;

namespace OutOfTimePrototype.Services.Interfaces;

public interface ILectureHallService
{
    Task<List<LectureHall>> GetAllUnoccupied(int timeSlotNumber, DateTime date);
    Task<List<LectureHall>> GetByBuilding(Guid hostBuildingId);
    Task<Result> CreateLectureHall(LectureHallCreateDto hallDto);
    Task<Result> EditLectureHall(Guid id, LectureHallUpdateDto hallUpdateDto);
}