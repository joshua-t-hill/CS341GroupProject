using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
/**
 * Author: Alex Ceithamer
 */
namespace CS341GroupProject
{
    public partial class UserSearchPage : ContentPage
    {
        public ObservableCollection<User> AllUsers { get; private set; }
        public ObservableCollection<User> FilteredUsers { get; private set; }

        public UserSearchPage()
        {
            InitializeComponent();
            /*
             * TODO: Implement a way to get the list of users from the database. Refer to BannedUsersPage.xaml.cs for additional information/thoughts.
             */

            // Sample data
            AllUsers = new ObservableCollection<User>
            {
                new User { Name = "Alice" },
                new User { Name = "Bob" },
                new User { Name = "Charlie" },
                // ... other users
            };

            FilteredUsers = new ObservableCollection<User>(AllUsers);
            UsersListView.ItemsSource = FilteredUsers;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTerm = e.NewTextValue;

            FilteredUsers.Clear();
            foreach (var user in AllUsers.Where(u => u.Name.ToLower().Contains(searchTerm.ToLower())))
            {
                FilteredUsers.Add(user);
            }
        }

        private void OnUserSelected(object sender, SelectedItemChangedEventArgs e)
        {
            BanButton.IsEnabled = e.SelectedItem != null;
        }

        private async void OnBanButtonClicked(object sender, EventArgs e)
        {
            var selectedUser = UsersListView.SelectedItem as User;
            if (selectedUser != null)
            {
                bool confirm = await DisplayAlert("Confirmation", $"Are you sure you want to ban {selectedUser.Name}?", "Yes", "No");
                if (confirm)
                {
                    // TODO: Add code to ban the user, such as updating your database
                    AllUsers.Remove(selectedUser);
                    FilteredUsers.Remove(selectedUser);
                    UsersListView.SelectedItem = null;
                }
            }
        }
    }

    /*
     * Refer to BannedUsersPage.xaml.cs for additional information/thoughts on user object class
     */
    public class User
    {
        public string Name { get; set; }
    }
}
