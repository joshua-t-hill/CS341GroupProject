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
            map.Pins.Add
            (
                new()
                {
                    Label = pd.Id.ToString(),
                    Address = "",
                    Type = PinType.SavedPin,
                    Location = new(pd.Latitude, pd.Longitude)
                }
            );
        }
    }

}