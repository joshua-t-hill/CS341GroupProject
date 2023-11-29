namespace CS341GroupProject.Pages;

public partial class SetNewPasswordPage : ContentPage
{
	public SetNewPasswordPage()
	{
		InitializeComponent();
	}
    
    /// <summary>
    /// Requires user to change password if they currently have a temporary password
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void OnResetPasswordClicked(System.Object sender, System.EventArgs e)
    {
        // Check if the two passwords entered match
        if (NewPasswordENT.Text.Equals(ConfirmPasswordENT.Text))
        {
            // Update their user in the database
            String username = SecureStorage.GetAsync("username").Result;
            bool success = MauiProgram.BusinessLogic.ChangeUserPassword(username, NewPasswordENT.Text, false);

            if (!success)
            {
                await DisplayAlert("Something went wrong.", "Try to reset the password again.", "OK");
            }

            await DisplayAlert("Password changed!", "Logging you in now...", "OK");
            
            // Navigate to tabbed map page
            Application.Current.MainPage = new AppShell();

        } else
        {
            await DisplayAlert("New passwords don't match", "Please try again.", "OK");
        }
    }
}