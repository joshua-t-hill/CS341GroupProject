using CS341GroupProject.Model;

namespace CS341GroupProject
{
    public class Post
    {
        String username;
        String plantGenus;
        String plantSpecies;
        String notes;
        Guid photoId;
        String plant;
        public String Username 
        { 
            get { return username; } 
            set { username = value; }
        }

        // keeping these two for now because they are used in the CommunityFeedPage
        // can edit the get and sets later if we want to keep these
        public ImageSource Photo 
        {
            get 
            {
                Photo photo = MauiProgram.BusinessLogic.SelectPhoto(photoId);
                ImageSource imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(photo.ImageData));
                return imageSource;
            }
        }
        public String Plant { get { return plantGenus + " " + plantSpecies; } set { plant = value; } }

        public String PlantGenus
        {
            get { return plantGenus; }
            set { plantGenus = value; }
        }
        public String PlantSpecies
        {
            get { return plantSpecies; }
            set { plantSpecies = value; }
        }
        public String Notes 
        { 
            get { return notes; }
            set { notes = value; }
        }
        public Guid PhotoId
        {
            get { return photoId; }
            set { photoId = value; }
        }

        public Post()
        {
            // to make Feed Page work
        }
        public Post(String username, String genus, String species, String notes, Guid photoId)
        {
            Username = username;
            PlantGenus = genus;
            PlantSpecies = species;
            Notes = notes;
            PhotoId = photoId;
        }
    }
}
