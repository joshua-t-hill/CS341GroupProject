namespace CS341GroupProject;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

		BindingContext = MauiProgram.BusinessLogic;
	}

	void OnLoginBtnClicked(object sender, EventArgs e)
	{
		Boolean success = MauiProgram.BusinessLogic.ConfirmLogin(UsernameENT.Text, PasswordENT.Text);

		if (!success)
		{
			DisplayAlert("Oops!", "Username or Password was incorrect.", "OK");
			return;
		}

		// Navigate to main page (AppShell?)
	}

	void NewUserTapped(object sender, TappedEventArgs args)
	{
		// await Navigation.PushAsync(new NewUserPage());
		throw new NotImplementedException();
	}

	void ForgotPasswordTapped(object sender, TappedEventArgs args)
	{
		// await Navigation.PushAsync(new ForgotPasswordPage());
		throw new NotImplementedException();
	}
}