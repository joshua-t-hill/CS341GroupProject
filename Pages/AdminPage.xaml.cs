
namespace CS341GroupProject
{
    public partial class AdminPage : ContentPage
    {

        public AdminPage()
        {
            InitializeComponent();
        }

        private async void OnBanListClickedAsync(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("BannedUsers");

        }

        private async void OnSearchClickedAsync(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("UserSearch");
        }

    }
}