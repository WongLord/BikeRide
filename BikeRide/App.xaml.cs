namespace BikeRide;

public partial class App : Application
{
	public App()
	{
		//Register Syncfusion license
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTI3NDQ4NEAzMjMwMmUzNDJlMzBWcXhLOGwrNXlkYmhvemhaNmVackk1d2lZY1Nvajc5U0h0N0RuZ0l2VSs4PQ==");

        InitializeComponent();

		MainPage = new AppShell();
	}
}
