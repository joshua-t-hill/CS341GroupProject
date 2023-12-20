
using System.Collections.ObjectModel;

namespace CS341GroupProject
{
    public partial class AppShell : Shell
    {
        private bool _isAdminVisible;
        public bool IsAdminVisible
        {
            get => _isAdminVisible;
            set
            {
                _isAdminVisible = value;
                OnPropertyChanged(nameof(IsAdminVisible));
            }
        }
        //used for thread synchronization. (False indicates that we aren't signaling the event on creation)
        public static ManualResetEvent PostsLoaded = new ManualResetEvent(false); 
        
        public AppShell()
        {
            InitializeComponent();

            //Declaring routing for non-tabbed pages here properly sets up the navigation stack
            Routing.RegisterRoute("Login", typeof(LoginPage));
            Routing.RegisterRoute("PostDetails", typeof(PostDetailsPage));
            Routing.RegisterRoute("Camera/AddPlant", typeof(AddPlantPage));
            Routing.RegisterRoute("EmailAddressConfirmation", typeof(EmailAddressConfirmationPage));
            Routing.RegisterRoute("FilterData", typeof(FilterDataPage));
            Routing.RegisterRoute("Admin/BannedUsers", typeof(BannedUsersPage));
            Routing.RegisterRoute("Admin/UserSearch", typeof(UserSearchPage));


            BindingContext = this;
        }

        /// <summary>
        /// Show the appropriate page based on the user's authentication status.
        /// </summary>
        /// <param name="isLoggedIn"></param>
        public void UpdateAuthenticationStatus(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                //Load first page of posts in background thread
                Task.Run(() =>
                {
                    MauiProgram.BusinessLogic.DynamicSelectPosts(1);
                    //signal the ManualResetEvent that the posts have been loaded
                    PostsLoaded.Set();
                });

                IsAdminVisible = MauiProgram.BusinessLogic.IsAdmin;
                // Navigate to main app
                GoToAsync("//Map");
            }
            else
            {
                // Navigate to login
                GoToAsync("//login");
            }
            /*
             * NOTE:
             * // Perform logout logic...
               // Update Shell
               if (Application.Current.MainPage is AppShell shell)
               {
                   shell.UpdateAuthenticationStatus(false);
               }
             */
        }

        /// <summary>
        /// Future class to handle persistent authentication.
        /// FIXME: implement this class
        /// </summary>
        private void CheckAuthenticationStatus()
        {
            // Determine if the user is logged in
            // This could be checking a token, a static property, etc.
            bool isLoggedIn = false;/* Logic to check if user is logged in */

            if (isLoggedIn)
            {
                // Navigate to main app
                GoToAsync("//Map");
            }
            else
            {
                // Navigate to login
                GoToAsync("//login");
            }
        }

        
    }
}