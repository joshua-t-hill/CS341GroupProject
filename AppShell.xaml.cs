
namespace CS341GroupProject
{
    public partial class AppShell : Shell
    {
        public bool IsAdminVisible { get; set; }
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

            BindingContext = this;
        }
    }
}