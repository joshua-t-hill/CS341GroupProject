using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
/**
 * Author: Joshua T. Hill
 * Map screen integrating google maps API.
 */
namespace CS341GroupProject;
public partial class MapPage : ContentPage
{

    public MapPage()
    {
        Location location = new Location(44.0263, -88.5507);
        MapSpan mapSpan = new MapSpan(location, 0.01, 0.01);
        Map map = new(mapSpan);

        Content = map;
    }

}