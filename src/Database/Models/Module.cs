using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace Database.Models;

public class Module
{
    [Key]
    public int Id { get; set; }

    public int RepositoryId { get; set; }

    public string Path { get; set; } = string.Empty;

    public string[] Providers { get; set; } = [];

    public string? DiagnosticMessages { get; set; }

    public (string Name, string Source)[] ModuleCalls { get; set; } = [];
}
