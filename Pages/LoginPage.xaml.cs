namespace CS341GroupProject;
using CS341GroupProject.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

		BindingContext = MauiProgram.BusinessLogic;

		// Hide the navigation bar
		NavigationPage.SetHasNavigationBar(this, false);

        // Hide the back button
        NavigationPage.SetHasBackButton(this, false);
    }

	async void OnLoginBtnClicked(object sender, EventArgs e)
	{
		Model.LoginError result = MauiProgram.BusinessLogic.ConfirmLogin(UsernameENT.Text, PasswordENT.Text);

		if (result == Model.LoginError.IncorrectInput)
		{
            await DisplayAlert("Oops!", "Username or Password was incorrect.", "OK");
			return;
		}
		else if (result == Model.LoginError.UserBanned)
		{
            await DisplayAlert("Oops!", "This account has been banned.", "OK");
            return;
        }

		// Navigate to AppShell
		await Navigation.PushAsync(new AppShell());

		// Remove the LoginPage from the Navigation stack
		Navigation.RemovePage(this);
	}

	async void NewUserTapped(object sender, TappedEventArgs args)
	{
		await Navigation.PushAsync(new CreateAccountPage());
	}

	void ForgotPasswordTapped(object sender, TappedEventArgs args)
	{
		// await Navigation.PushAsync(new ForgotPasswordPage());
		throw new NotImplementedException();
	}
}