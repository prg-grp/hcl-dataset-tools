using System.Text.Json.Serialization;

namespace RepositorySearcher.GitHub;

public record ApiResult<T>(
    [property: JsonPropertyName("total_count")]
    int TotalCount,
    List<T> Items);
