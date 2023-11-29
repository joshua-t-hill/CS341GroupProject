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
        Location location = new(Constants.MAP_STARTING_LATITUDE, Constants.MAP_STARTING_LONGITUDE); //NEED FIX -- have to change to get user location on startup instead of a default location
        //Location location = GetCurrentLocation().Result;  //not working at all, crashes program after giving permission to use location services.
        //Location location = Geolocation.GetLocationAsync().Result;

        MapSpan mapSpan = new(location, 0.01, 0.01);
        Map map = new(mapSpan);

        PopulateMapWithPins(map);

        Content = map;
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
            //Sets the behavior for the event when a pin is tapped on the map
            //pd.MarkerClicked += async (s, args) =>
            //{
            //    string plantName = $"Name: {pd.Genus} {pd.Epithet}";
            //    await DisplayAlert("Pin Clicked", plantName, "Ok");
            //};

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

    public async Task<Location> GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token); //crashes here

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

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }

}