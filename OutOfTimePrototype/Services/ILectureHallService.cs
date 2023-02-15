using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Services;

public interface ILectureHallService
{
    Task<List<LectureHall?>> GetFreeLectureHalls(int timeSlotNumber, DateTime date);
    Task<List<LectureHall>> GetLectureHallsByBuilding(Guid hostBuildingId);
    Task Create(LectureHallDTO hallDto);
    Task Update(LectureHallUpdateModel hallUpdateModel);
}