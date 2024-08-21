using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using CsvHelper;

using Database;

using RepositorySearcher;
using RepositorySearcher.GitHub;

using Shared;

using Spectre.Console;

AnsiConsole.MarkupLine("[green]Fetch HCL repositories from GitHub[/]");

var qFilePath = Environment.GetEnvironmentVariable("QUERY_FILE") ?? "./queries.csv";
var oLFilePath = Environment.GetEnvironmentVariable("OVER_LIMIT_FILE") ?? "./over_limit.csv";
var eFilePath = Environment.GetEnvironmentVariable("ERROR_FILE") ?? "./errors.csv";
AnsiConsole.MarkupLine("Fetching data and store it with the following settings:");
AnsiConsole.MarkupLineInterpolated($"[yellow]DB Directory: {Environment.GetEnvironmentVariable("DB_DIR")}[/]");
AnsiConsole.MarkupLineInterpolated($"[yellow]Query File: {qFilePath}[/]");
AnsiConsole.MarkupLineInterpolated($"[yellow]Over Limit File: {oLFilePath}[/]");
AnsiConsole.MarkupLineInterpolated($"[yellow]Error File: {eFilePath}[/]");

await using var db = new DatasetContext();
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitHubRepositorySearcher", "1.0"));
if (Environment.GetEnvironmentVariable("GITHUB_TOKEN") is { } token)
{
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}
else
{
    throw new ArgumentException("No Github Token provided.");
}

using var queryFile = new CsvReader(new StreamReader(File.OpenRead(qFilePath)), CultureInfo.InvariantCulture);
await using var overLimitFile =
    new CsvWriter(new StreamWriter(File.Create(oLFilePath)), CultureInfo.InvariantCulture);
await using var errFile = new CsvWriter(new StreamWriter(File.Create(eFilePath)), CultureInfo.InvariantCulture);

await SpectreCommon
    .Create()
    .StartAsync(async ctx =>
    {
        var queries = queryFile.GetRecords<GitHubQuery>().Select(q => q.Query).ToList();
        var totalTask = ctx.AddTask("Execute GitHub Queries", maxValue: queries.Count);

        foreach (var query in queries)
        {
            var task = ctx.AddTask($"Fetch: {query}");

            try
            {
                string? nextPageUrl = null;
                do
                {
                    var request =
                        new HttpRequestMessage(HttpMethod.Get,
                            nextPageUrl ??
                            $"https://api.github.com/search/repositories?q={query}&per_page=100");
                    var response = await Send(request);
                    var result = await response.Content.ReadFromJsonAsync<ApiResult<Repository>>();
                    if (result is null)
                    {
                        task.StopTask();
                        break;
                    }

                    if (result.TotalCount > 1000)
                    {
                        await overLimitFile.WriteRecordsAsync([new OverLimitQuery(query, result.TotalCount)]);
                    }

                    if (nextPageUrl is null)
                    {
                        task.MaxValue = (result.TotalCount / 100) + 1;
                    }

                    nextPageUrl = GetNextPageUrl(response);
                    await db.Repositories.AddRangeAsync(result.Items.Select(r => r.ToDbModel()));
                    await db.SaveChangesAsync();
                    task.Increment(1);
                } while (nextPageUrl is not null);
            }
            catch (Exception e)
            {
                await errFile.WriteRecordsAsync([new ErroredQuery(query, e.Message)]);
            }

            task.StopTask();
            totalTask.Increment(1);
        }

        totalTask.StopTask();
    });

AnsiConsole.MarkupLine("[green]Fetched all HCL repositories from GitHub[/]");

return 0;

async Task<HttpResponseMessage> Send(HttpRequestMessage message)
{
    var r = await client.SendAsync(message);
    // at max 30 requests per minute. So wait 2 seconds. (and 50 milliseconds for good measure)
    await Task.Delay(2050);
    return r;
}

string? GetNextPageUrl(HttpResponseMessage response)
{
    if (!response.Headers.TryGetValues("Link", out var links))
    {
        return null;
    }

    var link = links.First();
    var configuredLinks = link
        .Split(',')
        .Select(g => g.Trim().Split(';').Select(o => o.Trim().TrimStart('<').TrimEnd('>')))
        .ToDictionary(
            s => s.Last(),
            s => s.First()
        );

    return configuredLinks.TryGetValue("rel=\"next\"", out var n) ? n : null;
}
