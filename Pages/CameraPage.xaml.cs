using CS341GroupProject.Model;
using System.IO;

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

        TakePhoto();
	}

    public async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            // Convert the photo stream to byte array
            byte[] imageData = await ReadStream(photo.OpenReadAsync());

            MauiProgram.BusinessLogic.InsertPhoto(imageData);
            
            // For now, AddPlantPage does nothing; this just shows some navigation
            // AddPlantPage should probably take in the photo id instead
            await Navigation.PushAsync(new AddPlantPage(imageData));
        }
    }

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