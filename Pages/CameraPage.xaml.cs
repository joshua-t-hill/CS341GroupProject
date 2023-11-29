using CS341GroupProject.Model;

namespace CS341GroupProject;
/**
 * Author: Joshua T. Hill
 * Page that accesses native camera functionality to capture pictures of flora to upload to app DB w/ info.0
 */
public partial class CameraPage : ContentPage
{
	public CameraPage()
	{
		InitializeComponent();

        BindingContext = MauiProgram.BusinessLogic;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Call TakePhoto every time the page is loaded
        TakePhoto();
    }

    public async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo == null)
            {
                // Goes back to map page when camera X is clicked
                await Shell.Current.GoToAsync("///Map");
                return;
            }

            // Convert the photo stream to byte array
            byte[] imageData = await ReadStream(photo.OpenReadAsync());

            // Save the imageData in BusinessLogic to be displayed on the AddPlantPage
            MauiProgram.BusinessLogic.TempImageData = imageData;
            await Shell.Current.GoToAsync("AddPlant");
        }
    }

    // Reads in the photo data and converts it to a byte[]
    private async Task<byte[]> ReadStream(Task<Stream> task)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (Stream stream = await task)
            {
                await stream.CopyToAsync(ms);
            }
            return ms.ToArray();
        }
    }

}