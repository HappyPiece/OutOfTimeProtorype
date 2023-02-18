namespace OutOfTimePrototype.DAL.Models
{
    public class Cluster
    {
        public string Number { get; set; }
        public Cluster? SuperCluster { get; set; } = null;
    }
}
