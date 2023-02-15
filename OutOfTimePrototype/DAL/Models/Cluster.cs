namespace OutOfTimePrototype.DAL.Models;

public class Cluster
{
    public List<Class> Classes = new();
    public string Number { get; set; }
    public Cluster? SuperCluster { get; set; }
}