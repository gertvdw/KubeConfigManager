using Spectre.Console;

namespace kube.net.lib;

public class KubeConsole
{
    private readonly KubeConfig _config = new KubeConfig();
    private string _statusLine = "";
    
    /// <summary>
    /// Show a status line if something of note happened.
    /// Display a selection prompt with currently known actions and contexts
    /// </summary>
    public void MainLoop()
    {
        // Layout();
        AnsiConsole.Console.Clear();
        WriteLine($"[bold red]{_statusLine}[/]");
        WriteLine("");
        _statusLine = "";
        
        var context = PickContext();
        // <hacky shit>
        if (context.Equals("Import from clipboard"))
        {
            ImportConfig();
        }
        // </hacky shit>
        WriteLine($"You picked [red on grey]{context}[/]");
        var action = PickAction();
        switch (action)
        {
            case "Delete": DeleteFlow(context);
                break;
            case "Set as active":
                SetActiveFlow(context);
                break;
            case "Import from clipboard":
                ImportConfig();
                break;
            default: MainLoop();
                break;
        }
    }

    /// <summary>
    /// Display a selection prompt with all known contexts.
    /// </summary>
    /// <returns>The selected context as a string</returns>
    private string PickContext()
    {
        var prompt = new SelectionPrompt<string>()
            .Title("Manage your kubeconfig")
            .PageSize(12)
            .MoreChoicesText("Move up/down to reveal more options");
        
        var actionsChoice = prompt.AddChoice("Perform an action");
        actionsChoice.AddChild("Import from clipboard");

        var contextChoice = prompt.AddChoice("Contexts");
        foreach (var ctx in _config.Contexts())
        {
            contextChoice.AddChild(ctx);
        }

        var context = AnsiConsole.Prompt(
            prompt
        );
        return context;
    }

    /// <summary>
    /// Display a selection prompt
    /// </summary>
    /// <returns>The selected action as a string</returns>
    private string PickAction()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Pick an action to perform on this context")
                .AddChoices(new[]
                {
                    "Set as active",
                    "Delete",
                    "Go back"
                }));
    }

    /// <summary>
    /// Remove a context from the configuration.
    /// Prompts for confirmation first.
    /// </summary>
    /// <param name="context">The context name</param>
    private void DeleteFlow(string context)
    {
        if (Confirm())
        {
            _config.DeleteContext(context);
            StatusLine($"You just nuked [bold red on white]{context}[/]");
        }
        MainLoop();
    }

    /// <summary>
    /// Switch the active context to the context desired.
    /// This just sets the `current-context` bit in the config and writes it back.
    /// </summary>
    /// <param name="context">The context name</param>
    private void SetActiveFlow(string context)
    {
        _config.SetActiveContext(context);
        MainLoop();
    }

    /// <summary>
    /// Try to import a YAML config from clipboard.
    /// Invalid YAML (or YAML not resembling a kube config snippet) will show a warning in the statusline.
    /// A valid kube config snippet is merged with the current config.
    /// The new context is immediately visible in the list.
    /// </summary>
    private void ImportConfig()
    {
        try
        {
            var config = _config.LoadConfigFromClipboard();
            if (config != null) {
                var contextNames = _config.MergeConfig(config);

                if(contextNames.Count > 0)
                {
                    var commaSeparated = string.Join(", ", contextNames);
                    StatusLine($"Imported [bold green on black]{commaSeparated}[/]");
                }
            }
            else StatusLine("Valid YAML, but nothing returned");
        }
        catch (Exception e)
        {
            StatusLine("Can't parse contents from clipboard");
        }

        MainLoop();
    }

    /// <summary>
    /// Request confirmation before performing an action
    /// </summary>
    /// <returns>boolean</returns>
    private bool Confirm()
    {
        return AnsiConsole.Confirm($"Are you sure you want to perform this action?");
    }
    
    /// <summary>
    /// Write a markup line to the console
    /// </summary>
    /// <param name="args">A markup string</param>
    private void WriteLine(string args)
    {
        AnsiConsole.MarkupLine(args);
    }

    /// <summary>
    /// Set the status line (top line in console)
    /// </summary>
    /// <param name="args">A markup string</param>
    private void StatusLine(string args)
    {
        _statusLine = args;
    }
}