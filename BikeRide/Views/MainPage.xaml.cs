using BikeRide.ViewModels;

namespace BikeRide;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
        
        BindingContext = new MainViewModel();
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		
	}
}

