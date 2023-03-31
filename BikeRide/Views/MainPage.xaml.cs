using BikeRide.ViewModels;

namespace BikeRide;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
        try
        {
            InitializeComponent();

            BindingContext = new MainViewModel();
        }
        catch(Exception ex) 
        {
            BindingContext = new MainViewModel();
        }
    }

}

