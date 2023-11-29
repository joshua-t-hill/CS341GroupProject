using Microsoft.Maui.Controls.Maps;

namespace CS341GroupProject.Model;
public class PinData : Pin
{
    public long DatabaseId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Genus { get; set; }
    public string Epithet { get; set; }

    public PinData(long id, double latitude, double longitude, string genus, string epithet)
    {
        DatabaseId = id;
        Latitude = latitude; 
        Longitude = longitude; 
        Genus = genus; 
        Epithet = epithet;
    }

}
