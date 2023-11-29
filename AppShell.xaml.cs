
using System.Collections.ObjectModel;

namespace CS341GroupProject
{
    public partial class AppShell : Shell
    {
        public bool IsAdminVisible { get; set; }
        //used for thread synchronization. (False indicates that we aren't signaling the event on creation)
        public static ManualResetEvent PostsLoaded = new ManualResetEvent(false); 
        
        public AppShell()
        {
            InitializeComponent();

            IsAdminVisible = MauiProgram.BusinessLogic.IsAdmin;

            //Declaring routing for non-tabbed pages here properly sets up the navigation stack
            Routing.RegisterRoute("PostDetails", typeof(PostDetailsPage));
            Routing.RegisterRoute("Camera/AddPlant", typeof(AddPlantPage));
            Routing.RegisterRoute("EmailAddressConfirmation", typeof(EmailAddressConfirmationPage));
            Routing.RegisterRoute("FilterData", typeof(FilterDataPage));
            Routing.RegisterRoute("Admin/BannedUsers", typeof(BannedUsersPage));
            Routing.RegisterRoute("Admin/UserSearch", typeof(UserSearchPage));

            //Load first page of posts in background thread
            Task.Run(() =>
            {
                MauiProgram.BusinessLogic.DynamicSelectPosts(1);
                //signal the ManualResetEvent that the posts have been loaded
                PostsLoaded.Set();
            });


            BindingContext = this;
        }

        
    }
}