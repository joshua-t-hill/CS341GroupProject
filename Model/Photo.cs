
namespace CS341GroupProject.Model;

public class Photo
{
    Guid id;
    byte[] imageData;

    public Guid Id 
    { 
        get { return id; } 
        set { id = value; }
    }

    public byte[] ImageData
    {
        get { return imageData; }
        set {  imageData = value; }
    }

    public Photo(Guid id, byte[] imageData)
    {
        Id = id;
        ImageData = imageData;
    }
}
