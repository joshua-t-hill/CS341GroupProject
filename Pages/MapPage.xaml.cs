using CS341GroupProject.Model;
using System.Collections.ObjectModel;

namespace CS341GroupProject;
public partial class MapPage : ContentPage
{
    private ObservableCollection<PinData> customPins;
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation; //unused for now

    public MapPage()
    {
        InitializeComponent();
        GenerateMapAsync();
    }
    


    /// <summary>
    /// All the work that needs to be done to properly load the  map.
    /// </summary>
    private async void GenerateMapAsync()
    {
        //disable tab bar and start loading indicator
        Shell.SetTabBarIsVisible(this, false);
        loadingIndicator.IsVisible = true;
        loadingIndicator.IsRunning = true;

        var htmlSource = new HtmlWebViewSource
        {
            Html = LoadResourceText("CS341GroupProject.Pages.Subpages.osm.html")
        };
        webView.Source = htmlSource;

        var location = await GetCurrentLocation();
        SetMapView(location.Latitude, location.Longitude);
        await PopulateMapWithPins();

        //re-enable tab bar and stop loading indicator
        loadingIndicator.IsVisible = false;
        loadingIndicator.IsRunning = false;
        Shell.SetTabBarIsVisible(this, true);
    }

    /// <summary>
    /// Leverage phone GPS functionality to get user's current location as the map's initial focus area.
    /// Largely copied from the .NET MAUI documentation; will make more app-specific adjustments later and possibly move to its own BusinessLogic file later.
    /// </summary>
    /// <returns> An asynchronous task of type Location; the location map will open on </returns>
    private async Task<Location> GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true; //unused for now

            GeolocationRequest request = new(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
                return location;
            else
                return new(Constants.MAP_STARTING_LATITUDE, Constants.MAP_STARTING_LONGITUDE); //Else return default location (UW Oshkosh near Halsey)
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            CancelRequest(); //An error occurred, cancel request for location
            return new(Constants.MAP_STARTING_LATITUDE, Constants.MAP_STARTING_LONGITUDE); //return default location (UW Oshkosh near Halsey)
        }
        finally
        {
            _isCheckingLocation = false; //unused for now
        }
    }

    /// <summary>
    /// Add a pin to the map's internal collection of pins to display.
    /// </summary>
    /// <param name="pin"></param>
    private void AddPinToMap(PinData pin)
    {
        var photoBase64 = Convert.ToBase64String(pin.PhotoData);
        var photoUrl = $"data:image/png;base64,{photoBase64}";
        var script = $"addMarker({pin.Latitude}, {pin.Longitude}, '{pin.DatabaseId}', '{pin.Genus}', '{pin.Epithet}', '{photoUrl}')";
        webView.EvaluateJavaScriptAsync(script);
    }


    private void RemovePinFromMap(string pinId)
    {
        var script = $"removeMarker('{pinId}')";
        webView.EvaluateJavaScriptAsync(script);
    }

    private string LoadResourceText(string resourceID)
    {
        var assembly = this.GetType().Assembly;
        using (var stream = assembly.GetManifestResourceStream(resourceID))
        using (var reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    private void SetMapView(double latitude, double longitude)
    {
        var script = $"setMapView({latitude}, {longitude});";
        webView.EvaluateJavaScriptAsync(script);
    }

    /// <summary>
    /// Get the pins from DB and add to Map's internal collection of pins to display.
    /// Sets the event behaviors for each pin on load.
    /// </summary>
    private async Task PopulateMapWithPins()
    {
        customPins = await Task.Run(() => MauiProgram.BusinessLogic.CustomPins);

        foreach (var pin in customPins)
        {
            AddPinToMap(pin);
        }
    }

    //Need to look into functionality further down the line to make sure Cancellation works properly in different scenarios.
    private void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }

}