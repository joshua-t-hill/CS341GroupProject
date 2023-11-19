using CS341GroupProject.Model;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CS341GroupProject;
public partial class AddPlantPage : ContentPage
{
    private Photo NewPhoto = MauiProgram.BusinessLogic.Photo;
    public AddPlantPage()
	{
        InitializeComponent();

        // Convert the byte array to an ImageSource
        ImageSource imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(NewPhoto.ImageData));

        // Set the Image control source
        photoImage.Source = imageSource;
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

        await Shell.Current.GoToAsync("///CommunityFeed");
    }
}
