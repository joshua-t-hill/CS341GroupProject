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
        Map map = new();
        Content = map;
    }

}