using System.Collections.ObjectModel;
using CS341GroupProject.Model;
/**
 * Author: Alex Ceithamer
 */
namespace CS341GroupProject
{
    public partial class UserSearchPage : ContentPage
    {
        private ObservableCollection<User> AllUsers;
        public ObservableCollection<User> FilteredUsers { get; private set; }

        private User selectedUser;
        private User previousUser;

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
            UsersCollectionView.ItemsSource = FilteredUsers;

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
            foreach (var user in AllUsers.Where(u => u.Username.ToLower().Contains(searchTerm.ToLower())))
            {
                //add user to list if not banned
                if (!user.IsBanned) {
                    FilteredUsers.Add(user);
                }
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
            UsersCollectionView.SelectedItem = null;
            
            // selectedUser is already set to previousUser, so we just set selecteduser to null and reset CanBan for previousUser
            selectedUser = null;
            previousUser.CanBan = false;
        }

        /// <summary>
        /// This method enables the ban button for the selectedUser, disables 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserSelected(object sender, SelectionChangedEventArgs e)
        {
            // Unfocus the search bar
            UserSearchBar.Unfocus();

            // Get the currently selected user
            selectedUser = e.CurrentSelection.FirstOrDefault() as User;
            if (selectedUser != null)
            {
                // Reset the CanBan property for the previously selected user
                if (previousUser != null)
                {
                    previousUser.CanBan = false;
                }

                // Enable ban for the selected user
                selectedUser.CanBan = true;

                // Save the selected user as the previous user for when a different user is selected, we can set disable the ban button on this one
                previousUser = selectedUser;
            }
        }


        /// <summary>
        /// Ban functionality, When the ban button is clicked, We send a confirmation pop-up. If the user confirms, we remove the user from
        /// the all users list and the filtered users list, then reflect changes on the backend.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnBanButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            //var userToBan = button.CommandParameter as User;
            var selectedUser = UsersCollectionView.SelectedItem as User;
            if (selectedUser != null)
            {
                bool confirm = await DisplayAlert("Confirmation", $"Are you sure you want to ban {selectedUser.Username}?", "Yes", "No");
                if (confirm)
                {
                    //update IsBanned in the backend
                    User newUserInfo = new User(selectedUser.Username, selectedUser.Password, selectedUser.Email, true, false);
                    MauiProgram.BusinessLogic.UpdateUser(selectedUser, newUserInfo);

                    //remove user from our list
                    FilteredUsers.Remove(selectedUser);
                    AllUsers.Remove(selectedUser);

                    //reset variables to restore default state
                    UsersCollectionView.SelectedItem = null;
                    selectedUser.CanBan = false;
                }
            }
        } 

    }

}
