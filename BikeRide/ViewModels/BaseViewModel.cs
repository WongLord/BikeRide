using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BikeRide.ViewModels;



public class BaseViewModel : INotifyPropertyChanged
{
    BluetoothClient client = new();
    BluetoothDeviceInfo device = null;

    public ObservableCollection<BluetoothDeviceInfo> DeviceList { get; set; }

    BluetoothDeviceInfo deviceSelected;
    public BluetoothDeviceInfo DeviceSelected
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
    public ICommand CmdSend1 { get; set; }
    public ICommand CmdSend2 { get; set; }

    public BaseViewModel()
    {
        DeviceList = new ObservableCollection<BluetoothDeviceInfo>();

        CmdToggleConnection = new Command(async () => await ToggleConnection());

        CmdSearchForDevices = new Command(async () => await DiscoverDevices());

        CmdSend1 = new Command(async () => await SendData("1"));
        CmdSend2 = new Command(async () => await SendData("2"));

        Task.Run(async () => await DiscoverDevices());

    }

    private async Task SendData(string data)
    {
        var stream = client.GetStream();
        StreamWriter sw = new(stream, System.Text.Encoding.ASCII);
        await sw.WriteLineAsync(data);
        sw.Flush();
        //sw.Close();

        //client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
    }

    private async void ReceiveData()
    {
        var stream = client.GetStream();
        byte[] receive = new byte[1024];

        while (true)
        {
            Array.Clear(receive, 0, receive.Length);
            var readMessage = "";
            do
            {
                Thread.Sleep(1000);
                stream.Read(receive, 0, receive.Length);
                readMessage += Encoding.ASCII.GetString(receive);
            }
            while (stream.DataAvailable);

            switch (readMessage.FirstOrDefault())
            {
                case '1': await LeftTurn(); break;
                case '2': await RightTurn(); break;
                case '3': await Stop(); break;
            }
        }

        static async Task LeftTurn()
        {

        }

        static async Task RightTurn()
        {

        }

        static async Task Stop()
        {

        }
    }

    async Task DiscoverDevices()
    {
        try
        {

            //IsScanning = true;
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

            foreach (var dev in client.DiscoverDevices())
            {
                if (dev.DeviceName != null)
                {
                    if (dev.DeviceName.Contains("Bike"))
                    {

                        device = dev;
                        DeviceList.Add(device.de);
                        DeviceSelected = device;
                        IsDeviceListEmpty = false;
                        //IsScanning = false;
                        break;
                    }
                }
            }

            if (!device.Authenticated)
            {
                BluetoothSecurity.PairRequest(device.DeviceAddress, "0000");
            }
            device.Refresh();

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
                client.Close();
                IsConnected = false;
            }
            else
            {
                IsScanning = true;


                await Task.Run(() =>
                {
                    BluetoothAddress bta = new(Convert.ToUInt64("00220901163A", 16));
                    client.Connect(bta, BluetoothService.SerialPort);
                    DeviceSelected = device;
                    IsDeviceListEmpty = false;
                    IsScanning = false;


                    Thread tr = new(ReceiveData);
                    tr.IsBackground = true;
                    tr.Start();
                });

                IsConnected = client.Connected;

            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

    #region Permisos
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
    #endregion
}
