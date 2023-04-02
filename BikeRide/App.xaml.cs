namespace BikeRide;

public partial class App : Application
{
	public App()
	{
		//Register Syncfusion license
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTU3ODE1MkAzMjMxMmUzMTJlMzMzN2FXaG9VNy9YV01hQmFUMkVMNVgydWdING5MWS9oc2w1cVhJdDZ3Qm5MV0k9");

        InitializeComponent();

		MainPage = new AppShell();
	}
}
