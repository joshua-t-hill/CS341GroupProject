
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
            await Navigation.PushAsync(new BannedUsersPage());

        }

        private async void OnSearchClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserSearchPage());
        }

    }
}