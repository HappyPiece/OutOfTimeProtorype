namespace OutOfTimePrototype.DAL.Models
{
    public class LectureHall
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CampusBuilding HostBuilding { get; set; }
        public int Capacity { get; set; }
    }
}