using System.Text.Json.Serialization;

namespace ComputerMapping.Models;

public class Computer
{
    // We didn't have a PK originally, but SQL created one for us.
    //  ComputerId is Primary Key

    public int ComputerId { get; set; } 
    public string Motherboard { get; set; } = "";
    public int? CPUCores { get; set; } = 0;
    public bool HasWifi { get; set; }
    public bool HasLTE { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public decimal Price { get; set; }
    public string VideoCard { get; set; } = "";

    // JsonPropertyNames tell the model what to incoming Json property to map to this property.
    //  Can there be multiple?

/* Turning attributes on or off 
    [JsonPropertyName("computer_id")]
    public int ComputerId { get; set; } 
    [JsonPropertyName("motherboard")]
    public string Motherboard { get; set; } = "";
    [JsonPropertyName("cpu_cores")]
    public int? CPUCores { get; set; } = 0;
    [JsonPropertyName("has_wifi")]
    public bool HasWifi { get; set; }
    [JsonPropertyName("has_lte")]
    public bool HasLTE { get; set; }
    [JsonPropertyName("release_date")]
    public DateTime? ReleaseDate { get; set; }
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    [JsonPropertyName("video_card")]
    public string VideoCard { get; set; } = ""; */
}
