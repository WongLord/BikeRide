
using BikeRide.Models;

namespace BikeRide.ViewModels;

public class MainViewModel
{
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
}
