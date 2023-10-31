namespace CS341GroupProject.Model;
public class PinData
{
    public long Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Genus { get; set; }
    public string Epithet { get; set; }

    public PinData(long id, double latitude, double longitude, string genus, string epithet)
    {
        Id = id; 
        Latitude = latitude; 
        Longitude = longitude; 
        Genus = genus; 
        Epithet = epithet;
    }

    public override string ToString()
    {
        return $"Id:{Id} | Latitude:{Latitude} | Longitude:{Longitude} | Genus:{Genus} | Epithet:{Epithet}";
    }
}
