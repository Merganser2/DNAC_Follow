namespace ComputerInfo.Models;

public class Computer
{
    // We didn't have a PK originally, inserting data with Dapper, but SQL (?) created one for us; adding that
    //  field, ComputerId, to the Model
    public int ComputerId { get; set; } 
    public string Motherboard { get; set; } = "";
    public int? CPUCores { get; set; } = 0;
    public bool HasWifi { get; set; }
    public bool HasLTE { get; set; }
    public DateTime ReleaseDate { get; set; }
    public decimal Price { get; set; }
    public string VideoCard { get; set; } = "";
}
