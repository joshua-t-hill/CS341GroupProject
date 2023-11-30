using CommunityToolkit.Maui.Views;
using CS341GroupProject.Model;

namespace CS341GroupProject.Pages.Subpages;
public partial class PlantDetailsPopup : Popup
{
    public PlantDetailsPopup(PinData pin)
    {
        InitializeComponent();

        BindingContext = pin;
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}