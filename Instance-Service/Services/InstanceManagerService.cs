using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Instance_Service.Utils;
using Instance_Service.Config;
using Instance_Service.DB;
using MongoDB.Driver;
using System.Net.NetworkInformation;
using Shares.Helper;
using Microsoft.AspNetCore.Connections;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;

public class InstanceManagerService
{
    public static readonly Dictionary<string, (InstanceStatusResponse status, Process process)> INSTANCES
        = new();
    public static async Task<InstanceStatusResponse> StartNewInstanceAsync(InstanceRequest req)
    {
        string instanceName = req.InstanceName;
        var lastInstance = await DBHelper.GetLastStartedInstanceAsync();
        int? port = (lastInstance?.Port + 1) ?? 7778;
        int instanceId = (lastInstance?.InstanceId + 1) ?? 1000;
        //string instanceId = Guid.NewGuid().ToString();
        if (port == null)
            throw new Exception("No available port");

        var status = new InstanceStatusResponse
        {
            InstanceId = instanceId,
            InstanceName = instanceName,
            //IsRunning = true,
            PlayerCount = 0,
            Port = port.Value,
            ProcessId = 0,
            StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        //INSTANCES[instanceId.ToString()] = (status, process);

        Logger.Info($"Create Instance : Name = {status.InstanceName} | InstanceId = {status.InstanceId} | Port = {status.Port}");

        await DBHelper.SaveInstanceAsync(status); // คุณต้องสร้าง SaveInstanceAsync ใน Repo
        
        await Command.SendInstanceCommand(instanceName, new InstanceCommand
        {
            Action = "Launch",
            InstanceName = instanceName,
            InstanceId = instanceId,
            Port = port.Value
        }); 
        return status;
    }

    public static async Task<bool> JoinInstanceAsync(int instanceId)
    {
        var filter = Builders<InstanceStatusResponse>.Filter.Eq(x => x.InstanceId, instanceId);
        var update = Builders<InstanceStatusResponse>.Update.Inc(x => x.PlayerCount, 1);

        var result = await DBHelper.UpdateOneAsync(filter, update);

        if (result.MatchedCount > 0)
        {
            var instance = await DBHelper.GetInstanceByIdAsync(instanceId);
            if (instance != null)
            {
                Logger.Info($"Join Instance : Name = {instance.InstanceName} | P Count = {instance.PlayerCount} | Port = {instance.Port}");
            }
        }

        return result.MatchedCount > 0;
    }

    public static async Task<InstanceStatusResponse?> GetInstanceInfoAsync(int instanceId)
    {
        var collection = DBHelper.GetCollection();
        var filter = Builders<InstanceStatusResponse>.Filter.Eq(x => x.InstanceId, instanceId);
        var instance = await collection.Find(filter).FirstOrDefaultAsync();

        return instance;
    }

    public static async Task<bool> DestroyInstanceAsync(int instanceId)
    {

        try
        {
            var collection = DBHelper.GetCollection();

            // ค้นหา instance จาก DB
            var filter = Builders<InstanceStatusResponse>.Filter.Eq(x => x.InstanceId, instanceId);
            var instance = await collection.Find(filter).FirstOrDefaultAsync();

            if (instance == null)
            {
                Logger.Info($"Instance with ID {instanceId} not found.");
                return false;
            }

            // Kill process
            try
            {
                var process = Process.GetProcessById(instance.ProcessId);
                process.Kill(true);
                Logger.Info($"Killed process with PID {instance.ProcessId}");
            }
            catch (Exception ex)
            {
                Logger.Info($"Failed to kill process PID {instance.ProcessId}: {ex.Message}");
            }

            // ลบจาก DB
            var deleteResult = await collection.DeleteOneAsync(filter);

            Logger.Info($"Deleted instance from DB: InstanceId = {instance.InstanceId}");
            return deleteResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Logger.Info($"Error in DestroyInstanceAsync: {ex.Message}");
            return false;
        }
    }
    
    public static async Task<InstanceStatusResponse> StartNewInstanceAsync_Linux(InstanceRequest req)
    {
        string instanceName = req.InstanceName;
        string exePath;


        //Console.WriteLine($"test {instanceName}");
        var config = ConfigLoader.LoadInstanceConfig(instanceName);

        //Console.WriteLine($"asdasd {config} {instanceName}");

        var lastInstance = await DBHelper.GetLastStartedInstanceAsync();
        int? port = (lastInstance?.Port + 1) ?? 7778;
        int instanceId = (lastInstance?.InstanceId + 1) ?? 1000;
        //string instanceId = Guid.NewGuid().ToString();
        if (port == null)
            throw new Exception("No available port");

        if (Environment.GetEnvironmentVariable("DOCKER_ON") != "true")
        {
            exePath = @"A:\MMORPG\MMORPG\Binaries\WindowsServer\MMORPGServer.exe";
        }
        else
        {
            exePath = "/app/MMORPGServer/MMORPGServer.sh";
        }

       //exePath = @"A:\MMORPG\MMORPG\Binaries\WindowsServer\MMORPGServer.exe";
//
        string args = string.Join(" ", new[]
        {
            port.ToString(),
            instanceId.ToString(),
            //config.Enter.Map,
            //config.Enter.X.ToString(),
            //config.Enter.Y.ToString()
        });
//
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            }
        };
//
        process.Start();

        var status = new InstanceStatusResponse
        {
            InstanceId = instanceId,
            InstanceName = instanceName,
            //IsRunning = true,
            PlayerCount = 0,
            Port = port.Value,
            ProcessId = process.Id,
            StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        INSTANCES[instanceId.ToString()] = (status, process);

        Logger.Info($"Create Instance : Name = {status.InstanceName} | InstanceId = {status.InstanceId} | Port = {status.Port}");

        await DBHelper.SaveInstanceAsync(status); // คุณต้องสร้าง SaveInstanceAsync ใน Repo
        
        await Command.SendInstanceCommand(instanceName, new InstanceCommand
        {
            Action = "Launch",
            InstanceName = instanceName,
            InstanceId = instanceId,
            Port = port.Value
        }); 
        return status;
    }
    
    public static async Task<InstanceStatusResponse> StartNewInstanceAsync_(InstanceRequest req)
    {
        string instanceName = req.InstanceName;
        string exePath;


        //Console.WriteLine($"test {instanceName}");
        var config = ConfigLoader.LoadInstanceConfig(instanceName);

        //Console.WriteLine($"asdasd {config} {instanceName}");

        var lastInstance = await DBHelper.GetLastStartedInstanceAsync();
        int? port = (lastInstance?.Port + 1) ?? 7778;
        int instanceId = (lastInstance?.InstanceId + 1) ?? 1000;
        //string instanceId = Guid.NewGuid().ToString();
        if (port == null)
            throw new Exception("No available port");

        if (Environment.GetEnvironmentVariable("DOCKER_ON") != "true")
        {
            exePath = @"A:\MMORPG\MMORPG\Binaries\WindowsServer\MMORPGServer.exe";
        }
        else
        {
            exePath = "/app/MMORPGServer/MMORPGServer.exe";
        }

        exePath = @"A:\MMORPG\MMORPG\Binaries\WindowsServer\MMORPGServer.exe";
//
        string args = string.Join(" ", new[]
        {
            $"-ConsoleTitle=\"MMORPG Server | Port {port}\"",
            "-log",
            $"-port={port}",
            $"-instance_id={instanceId}",
            $"-map={config.Enter.Map}",
            $"-x={config.Enter.X}",
            $"-y={config.Enter.Y}"
        });
//
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            }
        };
//
        process.Start();

        var status = new InstanceStatusResponse
        {
            InstanceId = instanceId,
            InstanceName = instanceName,
            //IsRunning = true,
            PlayerCount = 0,
            Port = port.Value,
            ProcessId = process.Id,
            StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        INSTANCES[instanceId.ToString()] = (status, process);

        Logger.Info($"Create Instance : Name = {status.InstanceName} | InstanceId = {status.InstanceId} | Port = {status.Port}");

        await DBHelper.SaveInstanceAsync(status); // คุณต้องสร้าง SaveInstanceAsync ใน Repo
        
        await Command.SendInstanceCommand(instanceName, new InstanceCommand
        {
            Action = "Launch",
            InstanceName = instanceName,
            InstanceId = instanceId,
            Port = port.Value
        }); 
        return status;
    }

    // Add method for cleanup on app shutdown if needed
}
