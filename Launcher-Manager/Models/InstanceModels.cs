using MongoDB.Bson.Serialization.Attributes;


[BsonIgnoreExtraElements]
public class InstanceStatusResponse
{
    [BsonElement("instance_id")]
    public int InstanceId { get; set; }

    [BsonElement("instance_name")]
    public string InstanceName { get; set; }

    //[BsonElement("is_running")]
    //public bool IsRunning { get; set; }

    [BsonElement("player_count")]
    public int PlayerCount { get; set; }

    [BsonElement("port")]
    public int Port { get; set; }

    [BsonElement("processid")]
    public int ProcessId { get; set; }

    [BsonElement("started_at")]
    public string StartedAt { get; set; }
}

//public class InstanceCommand
//{
//    public string Action { get; set; } // "Launch" หรือ "Destroy"
//    public string InstanceName { get; set; }
//}

public class InstanceRequest
{
    public string InstanceName { get; set; }
    public int MaxPlayers { get; set; }
}
    

