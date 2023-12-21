using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

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

            try
            {
                // Convert the photo stream to byte array
                byte[] imageData = await ReadStream(photo.OpenReadAsync());

                // Compress the image data here
                byte[] compressedImageData = CompressImage(imageData);

                // Save the imageData in BusinessLogic to be displayed on the AddPlantPage
                MauiProgram.BusinessLogic.TempImageData = compressedImageData;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error processing photo", e.Message, "OK");
                return;
            }

            
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

    /// <summary>
    /// Compress image file data using SixLabors.ImageSharp to reduce the size of the image for increased performance
    /// </summary>
    /// <param name="imageData"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static byte[] CompressImage(byte[] imageData)
    {
        using var image = Image.Load(imageData);

        //resize if too large
        if (image.Width > 2000 || image.Height > 2000)
        {
            image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
        }
        //adjust quality until low enough
        byte[] catchVariable = null;
        for (int quality = 90; quality > 0; quality -= 10)
        {
            using var outputStream = new MemoryStream();
            var encoder = new JpegEncoder { Quality = quality };
            image.SaveAsJpeg(outputStream, encoder);

            if (outputStream.ToArray().Length < 100000)
            {
                return outputStream.ToArray();
            }
            catchVariable = outputStream.ToArray();
        }
        return catchVariable;
    }

}