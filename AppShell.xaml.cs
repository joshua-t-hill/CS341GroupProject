namespace CS341GroupProject
{
    public partial class AppShell : Shell
    {
        public bool IsAdminVisible { get; set; }
        public AppShell()
        {
            InitializeComponent();

            /*
             * Registering Routes to use for navigation from tabbed pages to other pages and back
             */
            Routing.RegisterRoute("AdminPage", typeof(AdminPage));
            Routing.RegisterRoute("BannedUsersPage", typeof(BannedUsersPage));
            Routing.RegisterRoute("UserSearchPage", typeof(UserSearchPage));
            Routing.RegisterRoute("MapPage", typeof(MapPage));
            Routing.RegisterRoute("CommunityFeedPage", typeof(CommunityFeedPage));
            Routing.RegisterRoute("CameraPage", typeof(CameraPage));
            Routing.RegisterRoute("PostDetailsPage", typeof(PostDetailsPage));
            Routing.RegisterRoute("ResetPasswordPage", typeof(ResetPasswordPage));
            //Routing.RegisterRoute("CreateAccountPage", typeof(CreateAccountPage));
            Routing.RegisterRoute("EmailAddressConfirmationPage", typeof(EmailAddressConfirmationPage));
            Routing.RegisterRoute("AddPlantPage", typeof(AddPlantPage));

            
            IsAdminVisible = MauiProgram.BusinessLogic.IsAdmin;

            BindingContext = this;
        }
    }
}