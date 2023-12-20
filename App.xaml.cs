﻿namespace CS341GroupProject
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;

            //We use NavigationPage to allow us to navigate between pages of login, create account, and reset password before using tabs later
            MainPage = new AppShell();
        }
    }
}