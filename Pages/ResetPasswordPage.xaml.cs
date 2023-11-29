namespace CS341GroupProject;
public partial class ResetPasswordPage : ContentPage
{
	public ResetPasswordPage()
	{
		InitializeComponent();

        BindingContext = MauiProgram.BusinessLogic;
    }

	/// <summary>
	/// ***NOT YET TESTED***
	/// Opens a user's default email app with a pre-loaded email
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	async void OnResetPasswordClicked(System.Object sender, System.EventArgs e)
	{
		if (UsernameENT.Text == null)
		{
			await DisplayAlert("", "Please enter your username.", "OK");
			return;
		}

        await MauiProgram.BusinessLogic.CreateResetPasswordEmail(UsernameENT.Text);

		// Pop back to the login page
        await Navigation.PopToRootAsync();
    }
}