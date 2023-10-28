using Microsoft.Maui.Controls.Maps;
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
        Location location = new(44.0263, -88.5507);
        MapSpan mapSpan = new(location, 0.01, 0.01);
        Map map = new(mapSpan);

        Pin pin1 = new()
        {
            Label = "Pin1",
            Address = "",
            Type = PinType.Generic,
            Location = new Location(44.023388, -88.553091)
        };
        Pin pin2 = new()
        {
            Label = "Pin2",
            Address = "",
            Type = PinType.Generic,
            Location = new Location(44.027935, -88.550843)
        };
        Pin pin3 = new()
        {
            Label = "Pin3",
            Address = "",
            Type = PinType.Generic,
            Location = new Location(44.026822, -88.557188)
        };

        map.Pins.Add(pin1);
        map.Pins.Add(pin2);
        map.Pins.Add(pin3);

        Content = map;
    }

}