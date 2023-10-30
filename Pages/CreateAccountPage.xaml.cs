using CS341GroupProject.Model;

namespace CS341GroupProject.Pages;

public partial class CreateAccountPage : ContentPage
{
	public CreateAccountPage()
	{
		InitializeComponent();

        BindingContext = MauiProgram.BusinessLogic;
    }

    async void OnNextBtnClicked(object sender, EventArgs e)
    {
        UserCreationError result = MauiProgram.BusinessLogic.CreateUser(UsernameENT.Text, PasswordENT.Text, EmailENT.Text);

        if (result != UserCreationError.NoError)
        {
            await DisplayAlert("Oops!", result.ToString(), "OK");
            return;
        }

        // Goes back to LoginPage for now, eventually should go to EmailAddressConfirmationPage
        await Navigation.PopAsync();
    }
}