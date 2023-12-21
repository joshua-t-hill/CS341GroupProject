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
		loadingIndicator.IsVisible = true;
		loadingIndicator.IsRunning = true;
		Model.LoginError result = await Task.Run(() => MauiProgram.BusinessLogic.ConfirmLogin(UsernameENT.Text, PasswordENT.Text));

		if (result == Model.LoginError.IncorrectInput)
		{
			loadingIndicator.IsVisible = false;
			loadingIndicator.IsRunning = false;
            await DisplayAlert("Oops!", "Username or Password was incorrect.", "OK");
			return;
		}
		else if (result == Model.LoginError.UserBanned)
		{
			loadingIndicator.IsVisible = false;
			loadingIndicator.IsRunning = false;
            await DisplayAlert("Oops!", "This account has been banned.", "OK");
            return;
		}
        else if (result == Model.LoginError.TempPasswordEntered)
        {
			loadingIndicator.IsVisible = false;
			loadingIndicator.IsRunning = false;
            await SecureStorage.SetAsync("username", UsernameENT.Text);
            await Navigation.PushAsync(new SetNewPasswordPage());
            return;
        }

        await SecureStorage.SetAsync("username", UsernameENT.Text);

        // Update Shell
        if (Application.Current.MainPage is AppShell shell)
        {
            shell.UpdateAuthenticationStatus(true);
            loadingIndicator.IsVisible = false;
            loadingIndicator.IsRunning = false;

        }
	}

    async void NewUserTapped(object sender, TappedEventArgs args)
	{
		await Navigation.PushAsync(new CreateAccountPage());
	}

	async void ForgotPasswordTapped(object sender, TappedEventArgs args)
	{
        await Navigation.PushAsync(new ResetPasswordPage());
    }
}