namespace RepositorySearcher;

public record GitHubQuery(string Query);

public record OverLimitQuery(string Query, int TotalCount);

public record ErroredQuery(string Query, string Message);
