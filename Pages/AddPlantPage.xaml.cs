using CS341GroupProject.Model;
namespace CS341GroupProject;
/**
 * Author: Samuel Ayoade
 */
public partial class AddPlantPage : ContentPage
{
    Photo newPhoto;
	public AddPlantPage(Photo photo)
	{
        InitializeComponent();
        newPhoto = photo;

        // Convert the byte array to an ImageSource
        ImageSource imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(photo.ImageData));

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
        Boolean success = MauiProgram.BusinessLogic.InsertPost(username, GenusENT.Text, SpeciesENT.Text, NotesENT.Text, newPhoto.Id);

        if (!success)
        {
            await DisplayAlert("Oops!", "Something went wrong, please try again.", "OK");
            return;
        }

        await DisplayAlert("", "Plant added!", "OK");
        // Takes the AddPlantPage off of the Navigation Stack
        await Navigation.PopToRootAsync();
        // BUG!!!! Opens Feed Page in Camera tab
        await Shell.Current.GoToAsync("CommunityFeedPage");
    }
}
