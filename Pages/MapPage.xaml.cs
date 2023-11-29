using CS341GroupProject.Model;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using Map = Microsoft.Maui.Controls.Maps.Map;
using CommunityToolkit.Maui.Views;
using CS341GroupProject.Pages.Subpages;

namespace CS341GroupProject;
public partial class MapPage : ContentPage
{
    private ObservableCollection<PinData> customPins;
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    public MapPage()
    {
        GenerateMapAsync();
    }

    private async void GenerateMapAsync()
    {
        Location location = await GetCurrentLocation();

        MapSpan mapSpan = new(location, 0.01, 0.01);

        Map map = new(mapSpan);

        PopulateMapWithPins(map);

        Content = map;
    }

    private async Task<Location> GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

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
            _isCheckingLocation = false;
        }
    }

    /// <summary>
    /// Get the pins from DB and add to Map's internal collection of pins to display.
    /// Sets the event behaviors for each pin on load.
    /// </summary>
    /// <param name="map"></param>
    private void PopulateMapWithPins(Map map)
    {
        customPins = MauiProgram.BusinessLogic.CustomPins;

        foreach (var pin in customPins)
        {
            //sets the behavior for the event when a pin's info window is tapped on the map
            //Has to be set in the CS file for the page as ShowPopupAsync is from Page.
            pin.InfoWindowClicked += async (s, args) =>
            {
                PlantDetailsPopup popup = new(pin);
                await this.ShowPopupAsync(popup);
            };

            map.Pins.Add(pin);
        }
    }

    //Need to look into functionality further down the line to make sure Cancellation works properly.
    private void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }

}