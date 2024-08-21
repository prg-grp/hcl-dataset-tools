using Spectre.Console;
using Spectre.Console.Rendering;

namespace Shared;

public class TaskCountColumn : ProgressColumn
{
    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    => Markup.FromInterpolated($"[yellow]{task.Value}/{task.MaxValue}[/]");
}
