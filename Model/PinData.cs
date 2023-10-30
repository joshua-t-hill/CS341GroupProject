namespace CS341GroupProject.Model;
public class PinData
{
    public string Uuid { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Genus { get; set; }
    public string Epithet { get; set; }

    public PinData(string uuid, double latitude, double longitude, string genus, string epithet)
    {
        Uuid = uuid; 
        Latitude = latitude; 
        Longitude = longitude; 
        Genus = genus; 
        Epithet = epithet;
    }
}
