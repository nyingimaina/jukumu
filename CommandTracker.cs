using System.Collections.Concurrent;

namespace Jukumu
{
    public static class CommandTracker
{
    // Store frequency of commands and options
    private static readonly ConcurrentDictionary<string, CommandUsage> CommandUsageData = new();

    // Log a command and its options
    public static void LogCommand(string commandName, string option)
    {
        // Update counts
        var usage = CommandUsageData.GetOrAdd(commandName, _ => new CommandUsage());
        usage.IncrementOption(option);
        usage.MarkAsDirty(); // Indicate sorting is needed
    }

    // Get sorted options based on usage
    public static string[] GetSuggestions<T>(string commandName, IEnumerable<T> defaultOptions, Func<T,string> nameRetriever = null!)
    {
        if(nameRetriever == null)
        {
            nameRetriever = (a) => a?.ToString()! ?? string.Empty;
        }
            var asArray = defaultOptions.Select(a => nameRetriever(a)).ToArray();
        if (CommandUsageData.TryGetValue(commandName, out var usage))
        {
            return usage.GetSortedOptions(asArray);
        }
        return asArray;
    }
}

// Helper class to track command options and frequency
public class CommandUsage
{
    private readonly ConcurrentDictionary<string, int> _optionFrequency = new();
    private string[]? _cachedSortedOptions;
    private bool _isDirty;

    public void IncrementOption(string option)
    {
        _optionFrequency.AddOrUpdate(option, 1, (_, count) => count + 1);
    }

    public void MarkAsDirty()
    {
        _isDirty = true;
    }

    public string[] GetSortedOptions(string[] defaultOptions)
    {
        // Only recompute sorted options if the data has changed
        if (_isDirty || _cachedSortedOptions == null)
        {
            _cachedSortedOptions = defaultOptions
                .OrderByDescending(option => _optionFrequency.GetOrAdd(option, 0))
                .ToArray();
            _isDirty = false;
        }

        return _cachedSortedOptions;
    }
}
}