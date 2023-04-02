﻿using BikeRide_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BikeRide_API.Controllers;

public class BikeRideController : ControllerBase
{
    [HttpGet("~/GetRideActions")]
    public IActionResult GetRideActions(int UserId)
    {
        return Ok(SQL.ExecuteScalar($"SP_GetRideActions {UserId}").ToString()); //JsonConvert.DeserializeObject<List<RideActions>>
    }

    [HttpGet("~/GetRideLocPoints")]
    public IActionResult GetRideLocPoints(int UserId)
    {
        return Ok(SQL.ExecuteScalar($"SP_GetRidesLocationPoints {UserId}").ToString()); //JsonConvert.DeserializeObject<List<RideLocationPoints>>
    }

    [HttpPost("~/PostNewRide")]
    public IActionResult PostNewRide([FromBody] Rides ride)
    {
        try
        {
            return Ok(SQL.ExecuteScalar($"SP_InNewRide {ride.UserId}, {ride.ActionId}"));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("~/PostNewRideAction")]
    public IActionResult PostNewRideAction([FromBody] RideActions action)
    {
        try
        {
            SQL.ExecuteQuery($"SP_InRideAction {action.UserId}, {action.ActionId}, {action.Latitude}, {action.Longitude}, {action.Speed}");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("~/PostLocPoint")]
    public IActionResult PostLocPoint([FromBody] RideLocationPoints point)
    {
        try
        {
            SQL.ExecuteQuery($"SP_InLocationPoint {point.RideId}, {point.Latitude}, {point.Longitude}");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
