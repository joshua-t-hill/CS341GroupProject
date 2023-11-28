using CS341GroupProject.Model;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CS341GroupProject;
public partial class AddPlantPage : ContentPage
{
    private Photo NewPhoto = MauiProgram.BusinessLogic.Photo;
    private Boolean ResetNavigationStack;
    public AddPlantPage()
	{
        InitializeComponent();

        // Convert the byte array to an ImageSource
        ImageSource imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(NewPhoto.ImageData));

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
        String username = SecureStorage.GetAsync("username").Result;
        Boolean success = MauiProgram.BusinessLogic.InsertPost(username, GenusENT.Text, SpeciesENT.Text, NotesENT.Text, NewPhoto.Id);

        if (!success)
        {
            await DisplayAlert("Oops!", "Something went wrong, please try again.", "OK");
            return;
        }

        await DisplayAlert("", "Plant added!", "OK");

        
        //Go to the Community Feed
        await Shell.Current.GoToAsync("///CommunityFeed");

    }
}
