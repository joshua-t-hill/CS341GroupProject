using System.Collections.ObjectModel;
using CS341GroupProject.Model;
using Microsoft.Maui.Controls;

namespace CS341GroupProject
{
    /**
    * Author: Alex Ceithamer
    */
    public partial class BannedUsersPage : ContentPage
    {
        private ObservableCollection<User> bannedUsers;
        public ObservableCollection<User> FilteredUsers { get; private set; }
        private User selectedUser;
        private User previousUser;

        /// <summary>
        /// Default constructor, populates the banned users list and binds it to the BannedUsersListView item source.
        /// </summary>
        public BannedUsersPage()
        {
            InitializeComponent();

            //Initialize ObservableCollection of banned users
            var Users = MauiProgram.BusinessLogic.Users;
            bannedUsers = new ObservableCollection<User>();
            foreach (var user in Users.Where(u => u.IsBanned == true))
            {
                bannedUsers.Add(user);
            }
            
            //populate filtered users ObservableCollection with all banned users
            FilteredUsers = new ObservableCollection<User>();
            foreach (var user in bannedUsers)
            {
                FilteredUsers.Add(user);
            }
            BannedUsersCollectionView.ItemsSource = FilteredUsers;

            BindingContext = this;
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
            foreach (var user in bannedUsers.Where(u => u.Username.ToLower().Contains(searchTerm.ToLower())))
            {
                FilteredUsers.Add(user);
            }
        }

        /// <summary>
        /// Unselect the current item when the user clicks on the search bar. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchBarFocused(object sender, FocusEventArgs e)
        {
            // Unselect the current item
            BannedUsersCollectionView.SelectedItem = null;

            // selectedUser is already set to previousUser, so we just set selecteduser to null and reset CanUnban for previousUser
            selectedUser = null;
            if (previousUser != null)
            {
                previousUser.CanUnban = false;
            }
        }

        private void OnUserSelected(object sender, SelectionChangedEventArgs e)
        {
            // Unfocus the search bar
            UserSearchBar.Unfocus();

            // Get the currently selected user
            selectedUser = e.CurrentSelection.FirstOrDefault() as User;
            if (selectedUser != null)
            {
                // Reset the CanUnban property for the previously selected user
                if (previousUser != null)
                {
                    previousUser.CanUnban = false;
                }

                // Enable Unban button for the selected user
                selectedUser.CanUnban = true;

                // Save the selected user as the previous user for when a different user is selected, we can set disable the Unban button on this one
                previousUser = selectedUser;
            }
        }

        /// <summary>
        /// Unban functionality, removes the selected user from the banned users list and updates the backend to reflect the change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnUnbanClicked(object sender, EventArgs e)
        {
            var selectedUser = BannedUsersCollectionView.SelectedItem as User;
            if (selectedUser != null)
            {
                bool confirm = await DisplayAlert("Confirmation", $"Are you sure you want to Unban {selectedUser.Username}?", "Yes", "No");
                if (confirm)
                {
                    //update the backend to reflect the change.
                    User oldUserInfo = selectedUser;
                    User newUserInfo = new User(oldUserInfo.Username, oldUserInfo.Password, oldUserInfo.Email, false, false);
                    MauiProgram.BusinessLogic.UpdateUser(oldUserInfo, newUserInfo);

                    //remove the user from lists
                    FilteredUsers.Remove(selectedUser);
                    bannedUsers.Remove(selectedUser);
                    

                    //reset variables to restore default state
                    BannedUsersCollectionView.SelectedItem = null;
                    selectedUser.CanUnban = true;
                }
            }
        }
    }

    
}
