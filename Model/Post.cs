using CS341GroupProject.Model;
using System.ComponentModel;

namespace CS341GroupProject
{
    public class Post : INotifyPropertyChanged
    {
        String username;
        String plantGenus;
        String plantSpecies;
        String notes;
        Guid photoId;
        String plant;
        ImageSource photo;
        public string Username
        {
            get => username;
            set
            {
                if (username != value)
                {
                    username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        // keeping these two for now because they are used in the CommunityFeedPage
        // can edit the get and sets later if we want to keep these
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
        public string Plant
        {
            get => plantGenus + " " + plantSpecies;
            set
            {
                if (plant != value)
                {
                    plant = value;
                    OnPropertyChanged(nameof(Plant));
                }
            }
        }
        public string PlantGenus
        {
            get => plantGenus;
            set
            {
                if (plantGenus != value)
                {
                    plantGenus = value;
                    OnPropertyChanged(nameof(PlantGenus));
                }
            }
        }
        public string PlantSpecies
        {
            get => plantSpecies;
            set
            {
                if (plantSpecies != value)
                {
                    plantSpecies = value;
                    OnPropertyChanged(nameof(PlantSpecies));
                }
            }
        }
        public string Notes
        {
            get => notes;
            set
            {
                if (notes != value)
                {
                    notes = value;
                    OnPropertyChanged(nameof(Notes));
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

                    // Load and set the image on a separate thread
                    // Technically best practice would be to run the task on an async method containing this code
                    Task.Run(() =>
                    {
                        Photo photo = MauiProgram.BusinessLogic.SelectPhoto(photoId);
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            this.photo = ImageSource.FromStream(() => new MemoryStream(photo.ImageData));
                            OnPropertyChanged(nameof(Photo));
                        });
                    });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
