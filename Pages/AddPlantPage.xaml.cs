using CS341GroupProject.Model;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CS341GroupProject;
public partial class AddPlantPage : ContentPage
{
    private byte[] imageData = MauiProgram.BusinessLogic.TempImageData;
    private Boolean ResetNavigationStack;

    //used for thread synchronization. (False indicates that we aren't signaling the event on creation)
    public static ManualResetEvent FirstPageLoaded = new ManualResetEvent(false);

    public AddPlantPage()
	{
        InitializeComponent();

        // Convert the byte array to an ImageSource
        ImageSource imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(imageData));

        // Set the Image control source
        photoImage.Source = imageSource;
    }

    /// <summary>
    /// Overriding OnAppearing to reset the navigation stack. Default .NET MAUI behavior is to keep the
    /// navigation stack so that when the user returns to this tab, they are returned to the same page.
    /// But we want the user to be returned to the Camera page when they return to this tab.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (ResetNavigationStack) { 
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("//Camera");
            });
        }
        else
        {
            ResetNavigationStack = true;
        }
    }


    async void OnAddPlantBtnClicked(System.Object sender, System.EventArgs e)
    {
        if (GenusENT.Text == null)
        {
            await DisplayAlert("", "Please enter a Genus.", "OK");
            return;
        }

        if (SpeciesENT.Text == null)
        {
            await DisplayAlert("", "Please enter a Species.", "OK");
            return;
        }

        // Add photo to the database using image data passed from the CameraPage
        Boolean insertPhotoSuccess = MauiProgram.BusinessLogic.InsertPhoto(imageData);

        if (!insertPhotoSuccess)
        {
            await DisplayAlert("Something went wrong.", "Please try to take another photo.", "OK");
            await Shell.Current.GoToAsync("//Camera");
        }

        // Get the newly added photo from the database (to use the id)
        Photo newPhoto = MauiProgram.BusinessLogic.SelectPhoto(imageData);

        if (newPhoto == null)
        {
            await DisplayAlert("Something went wrong.", "Please try to take another photo.", "OK");
            await Shell.Current.GoToAsync("//Camera");
        }

        Location location = await MauiProgram.BusinessLogic.GetCurrentLocation();
        Double latitude = location.Latitude;
        Double longitude = location.Longitude;
        // if the id is -1, the post does not have a valid location (handle later)
        long pin_id = -1;
        if (location != null)
        {
            Boolean pinInserted = MauiProgram.BusinessLogic.InsertPin(latitude, longitude, GenusENT.Text, SpeciesENT.Text);
            if (pinInserted)
            {
                // pin id will eventually be added into Post objects (if time permits)
                pin_id = MauiProgram.BusinessLogic.GetPinId(location.Latitude, location.Longitude, GenusENT.Text, SpeciesENT.Text);
            }
        }

        // Add the post to the database
        String username = SecureStorage.GetAsync("username").Result;
        Boolean success = MauiProgram.BusinessLogic.InsertPost(username, GenusENT.Text, SpeciesENT.Text, NotesENT.Text, newPhoto.Id);

        if (!success)
        {
            await DisplayAlert("Oops!", "Something went wrong, please try again.", "OK");
            return;
        }

        //indicate that this user has just posted, so FeedPage can load properly
        MauiProgram.BusinessLogic.JustAddedPost = true;
        //Load first page of posts in background thread
        _ = Task.Run(() =>
        {
            MauiProgram.BusinessLogic.DynamicSelectPosts(1);
            //signal the ManualResetEvent that the posts have been loaded
            FirstPageLoaded.Set();
        });
        await DisplayAlert("", "Plant added!", "OK");

        
        //Go to the Community Feed
        await Shell.Current.GoToAsync("///CommunityFeed");

    }

}
