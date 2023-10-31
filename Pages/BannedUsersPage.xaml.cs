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

        public BannedUsersPage()
        {
            InitializeComponent();

            //populate the banned users list
            bannedUsers = new ObservableCollection<User>();

            ObservableCollection<User> users = MauiProgram.BusinessLogic.Users;
            foreach (User user in users)
            {
                if (user.IsBanned)
                {
                    bannedUsers.Add(user);
                }
            }
            
            BannedUsersListView.ItemsSource = bannedUsers;
        }

        private void OnUnbanClicked(object sender, EventArgs e)
        {
            var selectedUser = BannedUsersListView.SelectedItem as User;
            if (selectedUser != null)
            {
                // Implement unbanning logic here
                // For example, removing the user from the list:
                bannedUsers.Remove(selectedUser);
                User oldUserInfo = selectedUser;
                User newUserInfo = new User(oldUserInfo.Username, oldUserInfo.Password, oldUserInfo.Email, false);
                MauiProgram.BusinessLogic.UpdateUser(oldUserInfo, newUserInfo);


                // If there is any backend or database, we'd also remove the ban there.
            }
        }
    }

    
}
