using Instance_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading.Tasks;
//[Authorize]
[ApiController]
[Route("instance-service")]
public class InstanceController : ControllerBase
{
    private readonly IHostApplicationLifetime _lifetime;


    //
    private readonly string ApiSecretKey;
    //var issuer = Environment.GetEnvironmentVariable("ISSUER");

    

    public InstanceController(IHostApplicationLifetime lifetime)
    {
        _lifetime = lifetime;

        ApiSecretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
    }

    private bool CheckSecretKey()
    {
        if (Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            if (authHeader.ToString() == $"Bearer {ApiSecretKey}")
                return true;
        }
        return false;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }

    [HttpPost("create-instance")]
    public async Task<IActionResult> CreateInstance([FromBody] InstanceRequest req)
    {
        try
        {
            if (!CheckSecretKey())
                return Unauthorized("Invalid API key");

            //Console.WriteLine($"asdasdasd { ApiSecretKey}");
            //var status = await InstanceManagerService.SendInstanceCommand(req.InstanceName);
            //return Ok(status);
            var status = await InstanceManagerService.StartNewInstanceAsync_Linux(req);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to create instance: {ex.Message}");
        }
    }

    [HttpPost("join-instance")]
    public async Task<IActionResult> JoinInstance([FromBody] JoinInstanceRequest req)
    {
        try
        {
            if (!CheckSecretKey())
                return Unauthorized("Invalid API key");

            var result = await InstanceManagerService.JoinInstanceAsync(req.InstanceId);
            if (!result)
                return NotFound("Instance not found");

            return Ok("Joined instance successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to join instance: {ex.Message}");
        }
    }

    [HttpPost("instance-info")]
    public async Task<IActionResult> GetInstanceInfo([FromBody] InstanceInfoRequest req)
    {
        try
        {
            if (!CheckSecretKey())
                return Unauthorized("Invalid API key");

            var instance = await InstanceManagerService.GetInstanceInfoAsync(req.InstanceId);
            if (instance == null)
                return NotFound("Instance not found");

            return Ok(instance);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to get instance info: {ex.Message}");
        }
    }

    [HttpPost("shutdown")]
    public IActionResult Shutdown()
    {
        if (!CheckSecretKey())
            return Unauthorized("Invalid API key");

        CleanupService.CleanupBeforeShutdown();
        _lifetime.StopApplication();

        return Ok("Instance-Service is shutting down...");
    }
}
