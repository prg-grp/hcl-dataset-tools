using System.Text.Json.Serialization;

namespace RepositorySearcher.GitHub;

public record Repository(
    int Id,
    string Name,
    [property: JsonPropertyName("full_name")]
    string FullName,
    [property: JsonPropertyName("created_at")]
    DateTime CreatedAt,
    [property: JsonPropertyName("pushed_at")]
    DateTime PushedAt,
    int Size,
    [property: JsonPropertyName("html_url")]
    string HtmlUrl,
    string? Description,
    bool Archived,
    [property: JsonPropertyName("git_url")]
    string GitUrl,
    [property: JsonPropertyName("ssh_url")]
    string SshUrl,
    [property: JsonPropertyName("clone_url")]
    string CloneUrl,
    string? Homepage,
    [property: JsonPropertyName("forks_count")]
    int ForkCount,
    [property: JsonPropertyName("stargazers_count")]
    int StarCount,
    License? License,
    string[] Topics
)
{
    public Database.Models.Repository ToDbModel() => new()
    {
        Id = this.Id,
        Name = this.Name,
        FullName = this.FullName,
        CreatedAt = this.CreatedAt,
        LatestCommitAt = this.PushedAt,
        Description = this.Description,
        SizeInKb = this.Size,
        License = this.License?.SpdxId,
        ForkCount = this.ForkCount,
        StarCount = this.StarCount,
        HtmlUrl = this.HtmlUrl,
        GitUrl = this.GitUrl,
        SshUrl = this.SshUrl,
        CloneUrl = this.CloneUrl,
        Homepage = this.Homepage is not "" and not null ? this.Homepage : null,
        Archived = this.Archived,
        Topics = this.Topics,
    };
}
