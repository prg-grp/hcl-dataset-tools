using System.Text.Json.Serialization;

namespace RepositorySearcher.GitHub;

public record License(
    [property: JsonPropertyName("spdx_id")]
    string SpdxId);
