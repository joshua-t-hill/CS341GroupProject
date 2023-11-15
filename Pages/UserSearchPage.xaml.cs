using System.Collections.ObjectModel;
using System.Linq;
using CS341GroupProject.Model;
using Microsoft.Maui.Controls;
/**
 * Author: Alex Ceithamer
 */
namespace CS341GroupProject
{
    public partial class UserSearchPage : ContentPage
    {
        private ObservableCollection<User> AllUsers;
        public ObservableCollection<User> FilteredUsers { get; private set; }

        /// <summary>
        /// Default constructor, populates the filtered users list and binds it to the UsersListView item source. Also populates the all users list.
        /// </summary>
        public UserSearchPage()
        {
            InitializeComponent();
            //get ObservableCollection of all users
            AllUsers = MauiProgram.BusinessLogic.Users;
            //populate filtered users ObservableCollection with all users
            FilteredUsers = new ObservableCollection<User>();
            foreach (var user in AllUsers.Where(u => u.IsBanned != true))
            {
                FilteredUsers.Add(user);
            }
            //bind filtered users ObservableCollection to UsersListView item source
            UsersListView.ItemsSource = FilteredUsers;
        }

        /// <summary>
        /// Updates the filtered users list to only contain users whose username contains the search term. This implements a live 
        /// search functionality, where the list is updated as the user types.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTerm = e.NewTextValue;

            //clear the list of users, and repopulate with the new searchTerm
            FilteredUsers.Clear();
            foreach (var user in AllUsers.Where(u => u.Username.ToLower().Contains(searchTerm.ToLower())))
            {
                //add user to list if not banned
                if (!user.IsBanned) {
                    FilteredUsers.Add(user);
                }
            }
        }

        /// <summary>
        /// When a user is selected, enable the ban button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserSelected(object sender, SelectedItemChangedEventArgs e)
        {
            BanButton.IsEnabled = e.SelectedItem != null;
        }

        /// <summary>
        /// Ban functionality, When the ban button is clicked, We send a confirmation pop-up. If the user confirms, we remove the user from
        /// the all users list and the filtered users list, then reflect changes on the backend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBanButtonClicked(object sender, EventArgs e)
        {
            var selectedUser = UsersListView.SelectedItem as User;
            if (selectedUser != null)
            {
                bool confirm = await DisplayAlert("Confirmation", $"Are you sure you want to ban {selectedUser.Username}?", "Yes", "No");
                if (confirm)
                {
                    //update IsBanned in the backend
                    User newUserInfo = new User(selectedUser.Username, selectedUser.Password, selectedUser.Email, true, false);
                    MauiProgram.BusinessLogic.UpdateUser(selectedUser, newUserInfo);
                    //remove user from our list
                    AllUsers.Remove(selectedUser);
                    FilteredUsers.Remove(selectedUser);
                    UsersListView.SelectedItem = null;
                }
            }
        }
    }

}
