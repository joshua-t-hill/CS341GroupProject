namespace CS341GroupProject
{
    public partial class AppShell : Shell
    {
        public bool IsAdminVisible { get; set; }
        public AppShell()
        {
            InitializeComponent();

            IsAdminVisible = MauiProgram.BusinessLogic.IsAdmin;

            BindingContext = this;
        }
    }
}