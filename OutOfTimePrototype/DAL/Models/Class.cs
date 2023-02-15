namespace OutOfTimePrototype.DAL.Models;

public class Class
{
    public Guid Id { get; set; }

    public ClassType? Type { get; set; }

    public TimeSlot TimeSlot { get; set; }

    public DateTime Date { get; set; }

    public LectureHall? LectureHall { get; set; }

    public Educator? Educator { get; set; }

    public List<Cluster> Clusters { get; set; } = new();
}