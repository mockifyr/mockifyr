using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Configuration
{
    [Key]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
}
