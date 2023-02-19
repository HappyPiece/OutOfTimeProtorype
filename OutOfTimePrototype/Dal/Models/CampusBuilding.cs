namespace OutOfTimePrototype.DAL.Models
{
    public class CampusBuilding
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Address { get; set; }

        public string Name { get; set; }

        public List<LectureHall> LectureHalls { get; set; }
    }
}
