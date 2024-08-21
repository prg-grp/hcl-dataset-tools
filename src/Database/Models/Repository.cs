using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Repository
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime LatestCommitAt { get; set; }

    public string LatestCommitSha { get; set; } = "TODO";

    public string? Description { get; set; }

    public string? License { get; set; }

    public int SizeInKb { get; set; }

    public int ForkCount { get; set; }

    public int StarCount { get; set; }

    public string HtmlUrl { get; set; } = string.Empty;

    public string GitUrl { get; set; } = string.Empty;

    public string SshUrl { get; set; } = string.Empty;

    public string CloneUrl { get; set; } = string.Empty;

    public string? Homepage { get; set; }

    public bool Archived { get; set; }

    public string[] Topics { get; set; } = [];
}
