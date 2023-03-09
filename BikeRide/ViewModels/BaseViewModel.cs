namespace BikeRide.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    int listenTimeout = 5000;

    protected ushort DEVICE_ID = 253;

    protected IAdapter adapter;
    protected IService service;

    public ObservableCollection<IDevice> DeviceList { get; set; }

    IDevice deviceSelected;
    public IDevice DeviceSelected
    {
        get => deviceSelected;
        set { deviceSelected = value; OnPropertyChanged(nameof(DeviceSelected)); }
    }

    bool isScanning;
    public bool IsScanning
    {
        get => isScanning;
        set { isScanning = value; OnPropertyChanged(nameof(IsScanning)); }
    }

    bool isConnected;
    public bool IsConnected
    {
        get => isConnected;
        set { isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
    }

    bool isDeviceListEmpty;
    public bool IsDeviceListEmpty
    {
        get => isDeviceListEmpty;
        set { isDeviceListEmpty = value; OnPropertyChanged(nameof(IsDeviceListEmpty)); }
    }

    public ICommand CmdToggleConnection { get; set; }

    public ICommand CmdSearchForDevices { get; set; }

    public BaseViewModel()
    {
        DeviceList = new ObservableCollection<IDevice>();

        adapter = CrossBluetoothLE.Current.Adapter;
        adapter.ScanTimeout = listenTimeout;
        adapter.ScanMode = ScanMode.LowLatency;
        adapter.DeviceDiscovered += AdapterDeviceDiscovered;

        CmdToggleConnection = new Command(async () => await ToggleConnection());

        CmdSearchForDevices = new Command(async () => await DiscoverDevices());
    }

    async Task ScanTimeoutTask()
    {
        await Task.Delay(listenTimeout);
        await adapter.StopScanningForDevicesAsync();
        IsScanning = false;
    }

    async void AdapterDeviceDiscovered(object sender, DeviceEventArgs e)
    {
        if (DeviceList.FirstOrDefault(x => x.Name == e.Device.Name) == null && !string.IsNullOrEmpty(e.Device.Name))
        {
            DeviceList.Add(e.Device);
        }

        //if (e != null && e.Device != null && !string.IsNullOrEmpty(e.Device.Name) && e.Device.Name.Contains("Gal"))
        //{
        //    await adapter.StopScanningForDevicesAsync();
        //    IsDeviceListEmpty = false;
        //    DeviceSelected = e.Device;
        //}
    }

    async Task DiscoverDevices()
    {
        try
        {
            PermissionStatus permissionStatus = await CheckBluetoothPermissions();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await RequestBluetoothPermissions();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert($"Bluetooth LE permissions", $"Bluetooth LE permissions are not granted.", "OK");
                    return;
                }
            }

            IsScanning = true;

            var tasks = new Task[]
            {
                    ScanTimeoutTask(),
                    adapter.StartScanningForDevicesAsync()
            };

            await Task.WhenAny(tasks);
        }
        catch (DeviceConnectionException ex)
        {
            Debug.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    async Task ToggleConnection()
    {
        try
        {
            if (IsConnected)
            {
                await adapter.DisconnectDeviceAsync(DeviceSelected);
                IsConnected = false;
            }
            else
            {
                await adapter.ConnectToDeviceAsync(DeviceSelected);
                IsConnected = true;
            }
        }
        catch (DeviceConnectionException ex)
        {
            Debug.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    protected int UuidToUshort(string uuid)
    {
        return int.Parse(uuid.Substring(4, 4), System.Globalization.NumberStyles.HexNumber); ;
    }

    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

    public async Task<PermissionStatus> CheckBluetoothPermissions()
    {
        PermissionStatus status = PermissionStatus.Unknown;
        try
        {
            status = await Permissions.CheckStatusAsync<BluetoothLEPermissions>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to check Bluetooth LE permissions: {ex.Message}.");
            await Shell.Current.DisplayAlert($"Unable to check Bluetooth LE permissions", $"{ex.Message}.", "OK");
        }
        return status;
    }

    public async Task<PermissionStatus> RequestBluetoothPermissions()
    {
        PermissionStatus status = PermissionStatus.Unknown;
        try
        {
            status = await Permissions.RequestAsync<BluetoothLEPermissions>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to request Bluetooth LE permissions: {ex.Message}.");
            await Shell.Current.DisplayAlert($"Unable to request Bluetooth LE permissions", $"{ex.Message}.", "OK");
        }
        return status;
    }
}
