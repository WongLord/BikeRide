using Syncfusion.Maui.Maps;

namespace BikeRide.ViewModels;

public class MainViewModel : BaseViewModel
{
    public MainViewModel(){
        adapter.DeviceConnected += AdapterDeviceConnected;
        adapter.DeviceDisconnected += AdapterDeviceDisconnected;

        CmdGetTemperature = new Command(async () => await GetTemperature());

        Rides.ForEach(r => { r.MapCenter = r.GpsPoints[(r.GpsPoints.Count / 2) - 1]; });
    }

    #region Home Tab Code
    public string ProfilePhotoSource { get; set; } = "dotnet_bot";
    public string ProfileName { get; set; } = "Leonardo";
    public string ProfileDetails { get; set; } = "Bike Ride Manager";

    public int LeftTurns { get; set; } = 89;
    public int AvgSpeed { get; set; } = 20;
    public int RightTurns { get; set; } = 71;

    public List<OverviewItem> OverviewItems { get; set; } = new List<OverviewItem>
    {
        new OverviewItem
        {
            IconSource = "turn_left",

            Title = "Left",
            Details = "Your Turn Left",

            Speed = 12.2
        },
        new OverviewItem
        {
            IconSource = "turn_left",

             Title = "Left",
            Details = "Your Turn Left",

            Speed = 14.2
        },
        new OverviewItem
        {
            IconSource = "turn_right",

            Title = "Right",
            Details = "Your Turn Right",

            Speed = 8.7
        },
        new OverviewItem
        {
            IconSource = "turn_left",

             Title = "Left",
            Details = "Your Turn Left",

            Speed = 10.4
        },
        new OverviewItem
        {
            IconSource = "turn_right",


             Title = "Right",
            Details = "Your Turn Right",

            Speed = 18.5
        }
    };

    #endregion

    #region Rides Tab Code

    //public MapLatLng MapCenter { get; set; } = new MapLatLng { Latitude = 31.294967072903898, Longitude = -110.94911890126043 };

    public List<RidesHistory> Rides { get; set; } = new List<RidesHistory>
    {
        new RidesHistory
        {
            DateTime = DateTime.Now,
            GpsPoints = new ObservableCollection<MapLatLng>
            {
                new MapLatLng { Latitude = 31.292736570443644, Longitude=-110.94944681504553 },
                new MapLatLng { Latitude = 31.29214433096709, Longitude=  -110.94933113906677 },
                new MapLatLng { Latitude = 31.292733306421663, Longitude=-110.94768725929922 },
                new MapLatLng{ Latitude=31.292360966046672, Longitude=-110.94639988349347},
                new MapLatLng{ Latitude=31.291761833444056, Longitude=-110.94554823493918},
                new MapLatLng{ Latitude=31.291460573238986, Longitude=-110.94563141921658}
            }
        },
        new RidesHistory
        {
            DateTime = DateTime.Now,
            GpsPoints = new ObservableCollection<MapLatLng>
            {
                new MapLatLng { Latitude = 31.292736570443644, Longitude=-110.94944681504553 },
                new MapLatLng { Latitude = 31.29214433096709, Longitude=  -110.94933113906677 },
                new MapLatLng { Latitude = 31.292733306421663, Longitude=-110.94768725929922 },
                new MapLatLng{ Latitude=31.292360966046672, Longitude=-110.94639988349347},
                new MapLatLng{ Latitude=31.291761833444056, Longitude=-110.94554823493918},
                new MapLatLng{ Latitude=31.291460573238986, Longitude=-110.94563141921658}
            }
        },
        new RidesHistory
        {
            DateTime = DateTime.Now,
            GpsPoints = new ObservableCollection<MapLatLng>
            {
                new MapLatLng { Latitude = 31.292736570443644, Longitude=-110.94944681504553 },
                new MapLatLng { Latitude = 31.29214433096709, Longitude=  -110.94933113906677 },
                new MapLatLng { Latitude = 31.292733306421663, Longitude=-110.94768725929922 },
                new MapLatLng{ Latitude=31.292360966046672, Longitude=-110.94639988349347},
                new MapLatLng{ Latitude=31.291761833444056, Longitude=-110.94554823493918},
                new MapLatLng{ Latitude=31.291460573238986, Longitude=-110.94563141921658}
            }
        }
    };

    #endregion

    #region Settings Bluetooh Tab Code
    ICharacteristic tempCharacteristic;

    string temperatureValue;
    public string TemperatureValue
    {
        get => temperatureValue;
        set { temperatureValue = value; OnPropertyChanged(nameof(TemperatureValue)); }
    }

    public ICommand CmdGetTemperature { get; set; }

    async void AdapterDeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
    {
        IsConnected = true;

        IDevice device = e.Device;

        var services = await device.GetServicesAsync();

        foreach (var serviceItem in services)
        {
            if (UuidToUshort(serviceItem.Id.ToString()) == DEVICE_ID)
            {
                service = serviceItem;
            }
        }

        tempCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.TEMPERATURE));
    }

    void AdapterDeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
    {
        IsConnected = false;
    }

    async Task GetTemperature()
    {
        try
        {
            TemperatureValue = Encoding.Default.GetString(await tempCharacteristic.ReadAsync()).Split(';')[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

   
    #endregion
}

public static class CharacteristicsConstants
{
    public const string ON = "73cfbc6f-61fa-4d80-a92f-eec2a90f8a3e";
    public const string OFF = "6315119d-d619-49bb-a21d-ef9e99941948";
    public const string PULSING = "d7551801-31fc-435d-a994-1e7f15e17baf";
    public const string BLINKING = "3a6cc4f2-a6ab-4709-a9bf-c9611c6bf892";
    public const string RUNNING_COLORS = "30df1258-f42b-4788-af2e-a8ed9d0b932f";

    public const string IS_CYCLING = "24517ccc-888e-4ffc-9da5-21884353b08d";
    public const string ANGLE = "5a0bb016-69ab-4a49-a2f2-de5b292458f3";

    public const string TEMPERATURE = "e78f7b5e-842b-4b99-94e3-7401bf72b870";
}