namespace OutOfTimePrototype.DAL.Models
{
    public class LectureHall
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public CampusBuilding HostBuilding { get; set; }
        public int Capacity { get; set; }
    }
}