using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace BikeRide.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public const int USER_ID = 1;
    public ICommand CmdNewRide { get; set; }
    public ICommand CmdStopRide { get; set; }

    public MainViewModel()
    {
        CmdNewRide = new Command(() => StartLocationUpdates());
        CmdStopRide = new Command(() => StopLocationUpdates());

        Updates = new();
        _locationService = new();

        IsBtnEnabled = true;
        IsNotBtnEnabled = !IsBtnEnabled;

        Task.Run(() => GetUserProfile(USER_ID)).Wait();
        Task.Delay(500).Wait();
        Task.Run(() => GetOverviewItems(USER_ID)).Wait();
        Task.Delay(500).Wait();
        Task.Run(() => GetRidesHistory(USER_ID)).Wait();
    }

    #region Home Tab Code
    public string ProfilePhotoSource { get; set; } 
    public string ProfileName { get; set; } 
    public string ProfileDetails { get; set; } 

    int _leftTurns;
    public int LeftTurns 
    { 
        get =>_leftTurns;
        set { _leftTurns = value; OnPropertyChanged(nameof(LeftTurns)); }
    }

    string _avgSpeed;
    public string AvgSpeed 
    {
        get => _avgSpeed;
        set { _avgSpeed = value; OnPropertyChanged(nameof(AvgSpeed)); } 
    }

    int _rightTurns;
    public int RightTurns 
    { 
        get => _rightTurns;
        set { _rightTurns = value; OnPropertyChanged(nameof(RightTurns)); } 
    }

    public async Task GetUserProfile(int userId)
    {
        HttpResponseMessage res = await ApiCalls.GETResponse("GetProfileInfo", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            UserProfile up = JsonConvert.DeserializeObject<UserProfile>(result.Replace("[", "").Replace("]", ""));

            ProfilePhotoSource = up.ProfilePhoto;
            ProfileName = up.Username;
            ProfileDetails = up.ProfileDescription;
            LeftTurns = up.LeftTurns;
            RightTurns = up.RightTurns;
            AvgSpeed = up.AvgSpeed.ToString("0.#");
        }
    }
    public List<OverviewItem> _overviewItems;
    public List<OverviewItem> OverviewItems 
    { 
        get => _overviewItems;
        set { _overviewItems = value; OnPropertyChanged(nameof(OverviewItems)); }
    }

    public async Task GetOverviewItems(int userId)
    {
        //OverviewItems = new();
        List<OverviewItem> items = new();
        HttpResponseMessage res = await ApiCalls.GETResponse("GetRideActions", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            List<BikeRideActions> actions = JsonConvert.DeserializeObject<List<BikeRideActions>>(result);


            foreach (var a in actions)
            {
                string Road = "Could not find road.";
                res = await ApiCalls.GETRoad(a.Latitude, a.Longitude);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    JObject o = JObject.Parse(json);
                    Road = (string)o.SelectToken("address.road");
                }

                items.Add(new OverviewItem
                {
                    IconSource = a.Actionname == "Turn Left" ? "turn_left" : a.Actionname == "Turn Right" ? "turn_right" : "stop_sign",
                    Title = a.Actionname,
                    Details = $"At {Road}{(Road.ToLower().Contains("calle") || Road.ToLower().Contains("avenida") ? "." : " road.")}",
                    Speed = a.Speed.ToString("0.#"),
                    ActionDate = a.ActionDate.ToString("d") == DateTime.Now.ToString("d") ? a.ActionDate.ToString("t") : a.ActionDate.ToString("g")
                });
            }
        }
        items.Reverse();
        OverviewItems = items;
    }
    #endregion

    #region Rides Tab Code

    bool isBtnEnabled;
    public bool IsBtnEnabled
    {
        get => isBtnEnabled;
        set { isBtnEnabled = value; OnPropertyChanged(nameof(IsBtnEnabled)); }
    }

    bool isNotBtnEnabled;
    public bool IsNotBtnEnabled
    {
        get => isNotBtnEnabled;
        set { isNotBtnEnabled = value; OnPropertyChanged(nameof(IsNotBtnEnabled)); }
    }

    public static int RideId { get; set; } = 0;

    List<RidesHistory> _ridesHistory;
    public List<RidesHistory> Rides { 
        get => _ridesHistory;
        set { _ridesHistory = value; OnPropertyChanged(nameof(Rides)); } 
    }

    public async Task GetRidesHistory(int userId)
    {
        List<RidesHistory> _rides = new();

        HttpResponseMessage res = await ApiCalls.GETResponse("GetRideLocPoints", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            List<RidesPoints> ridesPoints = JsonConvert.DeserializeObject<List<RidesPoints>>(result);

            var groupedRides = ridesPoints.GroupBy(r => r.StartDateTime).Select(g => g.ToList()).ToList();

            foreach (var g in groupedRides)
            {
                ObservableCollection<MapLatLng> Points = new();
                DateTime _startDateTime = DateTime.Now;
                g.ForEach((p) =>
                {
                    _startDateTime = p.StartDateTime;
                    Points.Add(new MapLatLng
                    {
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    });
                });

                _rides.Add(new RidesHistory
                {
                    DateTime = _startDateTime,
                    GpsPoints = Points
                });

                _rides.ForEach((p) => { p.MapCenter = p.GpsPoints[(p.GpsPoints.Count / 2) - 1]; });
            }
        }
        _rides.Reverse();
        Rides = _rides;
    }

    #region Codigo para tomar la ubicacion continuamente para los Rides
    private readonly Services.LocationService _locationService;

    bool _locationUpdatesEnabled;
    public bool LocationUpdatesEnabled
    {
        get => _locationUpdatesEnabled;
        set { _locationUpdatesEnabled = value; OnPropertyChanged(nameof(LocationUpdatesEnabled)); }
    }
    public ObservableCollection<object> Updates { get; }
    public void ChangeLocationUpdates()
    {
        LocationUpdatesEnabled = !LocationUpdatesEnabled;
        if (LocationUpdatesEnabled)
            StartLocationUpdates();
        else
            StopLocationUpdates();
    }

    public void StartLocationUpdates()
    {
        IsBtnEnabled = false;
        IsNotBtnEnabled = !IsBtnEnabled;
        Rides ride = new() { UserId = USER_ID, ActionId = (int)BikeAction.NewRide };
        using (var rdr = ApiCalls.POSTRequest("PostNewRide", ride))
        {
            RideId = Convert.ToInt32(rdr.ReadToEnd());
        }

        _locationService.LocationChanged += LocationService_LocationChanged;
        _locationService.Initialize();
    }

    public async void StopLocationUpdates()
    {
        _locationService.Stop();
        _locationService.LocationChanged -= LocationService_LocationChanged;

        try
        {
            Rides ride = new() { RideId = RideId, EndDateTime = DateTime.Now };
            ApiCalls.POSTRequest("EndRide", ride);

            List<RideCoordinates> points = new();
            foreach (var l in Updates)
            {
                var p = l as LocationModel;
                points.Add(new RideCoordinates { Latitude = p.Latitude, Longitude = p.Longitude });
            }

            RideLocationPoints rlp = new() { RideId = RideId, RideCoordinates = points };
            ApiCalls.POSTRequest("PostLocPoint", rlp);

            Updates.Clear();
            RideId = 0;

            await GetRidesHistory(USER_ID);
            IsBtnEnabled = true;
            IsNotBtnEnabled = !IsBtnEnabled;
        }
        catch(Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}\n\n{ex.InnerException}", "Ok");
        }
    }

    private void LocationService_LocationChanged(object sender, Models.LocationModel e)
    {
        if (e.Latitude != 0 && e.Longitude != 0)
            Updates.Add(e);
    }
    #endregion
    #endregion

    #region Settings Bluetooh Tab Code
    //Bluetooth setting en BaseViewModel
    #endregion
}