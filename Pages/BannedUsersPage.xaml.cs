using System.Collections.ObjectModel;
using CS341GroupProject.Model;
using Microsoft.Maui.Controls;
/**
 * Author: Alex Ceithamer
 */
namespace CS341GroupProject
{
    public partial class BannedUsersPage : ContentPage
    {
        public ObservableCollection<User> bannedUsers;

        /// <summary>
        /// Default constructor, populates the banned users list and binds it to the BannedUsersListView item source.
        /// </summary>
        public BannedUsersPage()
        {
            InitializeComponent();

            //Initialize ObservableCollection of banned users
            bannedUsers = new ObservableCollection<User>();
            //Populate banned users list
            ObservableCollection<User> users = MauiProgram.BusinessLogic.Users;
            foreach (User user in users)
            {
                if (user.IsBanned)
                {
                    bannedUsers.Add(user);
                }
            }
            //Bind banned users list to BannedUsersListView item source
            BannedUsersListView.ItemsSource = bannedUsers;
        }

        /// <summary>
        /// Unban functionality, removes the selected user from the banned users list and updates the backend to reflect the change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnbanClicked(object sender, EventArgs e)
        {
            var selectedUser = BannedUsersListView.SelectedItem as User;
            if (selectedUser != null)
            {
                //remove the user from the banned users list
                bannedUsers.Remove(selectedUser);

                //update the backend to reflect the change.
                User oldUserInfo = selectedUser;
                User newUserInfo = new User(oldUserInfo.Username, oldUserInfo.Password, oldUserInfo.Email, false, false);
                MauiProgram.BusinessLogic.UpdateUser(oldUserInfo, newUserInfo);
            }
        }
    }

    
}
