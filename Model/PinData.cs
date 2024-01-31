using Microsoft.Maui.Controls.Maps;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CS341GroupProject.Model;
public class PinData : Pin
{
    ImageSource photo;
    Guid photoId;
    public long DatabaseId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Genus { get; set; }
    public string Epithet { get; set; }
    public byte[] PhotoData { get; set; }
    public ImageSource Photo
    {
        get => photo;
        private set
        {
            if (photo != value)
            {
                photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }
    }
    public Guid PhotoId
    {
        get => photoId;
        set
        {
            if (photoId != value)
            {
                photoId = value;
                OnPropertyChanged(nameof(PhotoId));
                Photo photo = MauiProgram.BusinessLogic.SelectPhoto(photoId);
                    
                //set photo data so we can pass it to the plant pop-up page for map pins
                PhotoData = photo.ImageData;

                //set photo to an image source
                //this.photo = ImageSource.FromStream(() => new MemoryStream(photo.ImageData));

                OnPropertyChanged(nameof(Photo));
            }
        }
    }

    public PinData(long id, double latitude, double longitude, string genus, string epithet, Guid photoId)
    {
        DatabaseId = id;
        Latitude = latitude; 
        Longitude = longitude; 
        Genus = genus; 
        Epithet = epithet;
        PhotoId = photoId;
    }

}
