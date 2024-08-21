using Spectre.Console;

namespace Shared;

public static class SpectreCommon
{
    public static Progress Create() => AnsiConsole.Progress().HideCompleted(true)
        .Columns([
            new TaskDescriptionColumn(),
            new ProgressBarColumn(),
            new TaskCountColumn(),
            new PercentageColumn(),
            new ElapsedTimeColumn(),
            new SpinnerColumn(),
        ]);
}
