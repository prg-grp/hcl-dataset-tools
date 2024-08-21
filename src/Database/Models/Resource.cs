using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public enum ResourceType
{
    Managed,
    Data,
}

public class Resource
{
    [Key]
    public int Id { get; set; }

    public int ModuleId { get; set; }

    public ResourceType ResourceType { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;
}
