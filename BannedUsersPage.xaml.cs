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

            /*
             * TODO: Implement a way to get the list of banned users from the database.
             */

            //SAMPLE DATA
            BannedUsers = new ObservableCollection<BannedUser>
            {
                new BannedUser { Name = "User1" },
                new BannedUser { Name = "User2" }
                
            };
            BannedUsersListView.ItemsSource = BannedUsers;
        }

        private void OnUnbanClicked(object sender, EventArgs e)
        {
            var selectedUser = BannedUsersListView.SelectedItem as BannedUser;
            if (selectedUser != null)
            {
                // Implement unbanning logic here
                // For example, removing the user from the list:
                BannedUsers.Remove(selectedUser);

                // If there is any backend or database, we'd also remove the ban there.
            }
        }
    }

    /*FIXME: This class should be moved to a separate file perhaps? Or implemented in a different way (having a user class to store info from
     * the database with a 'bool banned' field, for example). This would only make sense if we need to pull user's info from the database for
     * another reason. Otherwise it would make sense to just querry the database for the banned users when enteirng this page.
     * 
     * Thought/update: UserSearchPage also needs a list of users (non-banned), so we could load a list of all users from the database when 
     * loading the admin page (1 list for normal users, another for banned users), and just have a User.java class that has a banned field.
    */
    public class BannedUser
    {
        public string Name { get; set; }
        public string Reason { get; set; }
    }
}
