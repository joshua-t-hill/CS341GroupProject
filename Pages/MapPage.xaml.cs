using CS341GroupProject.Model;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using Map = Microsoft.Maui.Controls.Maps.Map;
/**
 * Author: Joshua T. Hill
 * Map screen integrating google maps API.
 */
namespace CS341GroupProject;
public partial class MapPage : ContentPage
{
    private ObservableCollection<PinData> pinsData;
    public MapPage()
    {
        Location location = new(Constants.MAP_STARTING_LATITUDE, Constants.MAP_STARTING_LONGITUDE);
        MapSpan mapSpan = new(location, 0.01, 0.01);
        Map map = new(mapSpan);

        PopulateMapWithPins(map);

        Content = map;
    }

    private void PopulateMapWithPins(Map map)
    {
        pinsData = MauiProgram.BusinessLogic.PinsData;

        foreach (var pd in pinsData)
        {
            Pin pin = new Pin
            {
                Label = pd.Id.ToString(),
                Address = $"{pd.Genus} {pd.Epithet}",
                Type = PinType.SavedPin,
                Location = new(pd.Latitude, pd.Longitude),
                MarkerId = pd
            };

            //***Trying to access the PinData object saved under MarkerId doesn't seem to work when used with the MarkerClicked event.
            //***Not sure why; keeping for reference but implementing a work-around for now.
            //pin.MarkerClicked += async (s, args) =>
            //{
            //    PinData plantData = ((Pin)s).MarkerId as PinData;
            //    string plantName = $"Name: {plantData.Genus} {plantData.Epithet}";
            //    await DisplayAlert("Pin Clicked", plantName, "Ok");
            //};

            map.Pins.Add(pin);
        }
    }

}