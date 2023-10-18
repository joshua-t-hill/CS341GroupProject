using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
/**
 * Author: Alex Ceithamer
 */
namespace CS341GroupProject
{
    public partial class BannedUsersPage : ContentPage
    {
        public ObservableCollection<BannedUser> BannedUsers { get; private set; }

        public BannedUsersPage()
        {
            InitializeComponent();

            // Sample data
            BannedUsers = new ObservableCollection<BannedUser>
            {
                new BannedUser { Name = "User1" },
                new BannedUser { Name = "User2" }
                // Add more banned users as needed
            };

            BannedUsersListView.ItemsSource = BannedUsers;
        }

        private void OnUnbanClicked(object sender, EventArgs e)
        {
            var selectedUser = BannedUsersListView.SelectedItem as BannedUser;
            if (selectedUser != null)
            {
                // Implement your unbanning logic here
                // For example, removing the user from the list:
                BannedUsers.Remove(selectedUser);

                // If you have any backend or database, you'd also remove the ban there.
            }
        }
    }

    public class BannedUser
    {
        public string Name { get; set; }
        public string Reason { get; set; }
    }
}
