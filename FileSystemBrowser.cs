using Jukumu;
using Spectre.Console;

namespace Jukumu
{
    public static class FileSystemBrowser
    {
        private const string ParentDirectory = ".. (Parent Directory)";
        private const string SetDirectoryHere = ">Set Current Directory Here<";
        /// <summary>
        /// Allows the user to browse, preview, and select a directory or file with filters.
        /// </summary>
        /// <param name="initialDirectory">The directory to start browsing from. Defaults to the current directory.</param>
        /// <param name="fileFilter">Optional file filter (e.g., "*.cs" for C# files).</param>
        /// <param name="previewFiles">Set to true to enable file previewing.</param>
        /// <returns>The selected directory or file path, or null if canceled.</returns>
        public static string? Browse(string initialDirectory = "", string? fileFilter = null, bool previewFiles = false)
        {
            string currentDirectory = string.IsNullOrWhiteSpace(initialDirectory)
                ? Directory.GetCurrentDirectory()
                : initialDirectory;

            while (true)
            {
                // List directories and files
                var directories = Directory.GetDirectories(currentDirectory)
                    .Select(Path.GetFileName)
                    .OrderBy(name => name)
                    .ToList();

                var files = fileFilter != null
                    ? Directory.GetFiles(currentDirectory, fileFilter)
                        .Select(Path.GetFileName)
                        .OrderBy(name => name)
                        .ToList()!
                    : new List<string>();

                // Add navigation options
                directories.Insert(0, ParentDirectory);
                directories.Insert(0, SetDirectoryHere);
                var allChoices = directories.Concat(files).ToList();

                var selection = SelectionManager.SelectOption(
                    allChoices,
                    title: $"Browsing: [blue]{currentDirectory}[/]\nSelect a directory or file:",
                    a => a!.ToString(),
                    a => string.Empty
                );

                

                // Handle the user's selection
                if (selection == ParentDirectory)
                {
                    // Navigate up one level
                    currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                }
                else if (selection == SetDirectoryHere)
                {
                    // Return the current directory
                    return currentDirectory;
                }
                else if (directories.Contains(selection))
                {
                    // Navigate into the selected directory
                    currentDirectory = Path.Combine(currentDirectory, selection);
                }
                else if (files.Contains(selection))
                {
                    // Preview the file content if enabled
                    var filePath = Path.Combine(currentDirectory, selection);
                    if (previewFiles)
                    {
                        PreviewFile(filePath);
                    }

                    // Confirm file selection
                    if (SelectionManager.Confirm($"Do you want to select [green]{selection}[/]?"))
                    {
                        return filePath;
                    }
                }
            }
        }



        /// <summary>
        /// Previews the content of a file.
        /// </summary>
        /// <param name="filePath">The path to the file to preview.</param>
        private static void PreviewFile(string filePath)
        {
            // Limit the number of lines to preview
            const int previewLineCount = 10;

            AnsiConsole.MarkupLine($"[blue]Previewing file:[/] [green]{Path.GetFileName(filePath)}[/]");
            AnsiConsole.Write(new Rule());

            try
            {
                var lines = File.ReadLines(filePath).Take(previewLineCount).ToList();
                foreach (var line in lines)
                {
                    AnsiConsole.MarkupLine("[grey]{0}[/]", line.EscapeMarkup());
                }

                if (lines.Count >= previewLineCount)
                {
                    AnsiConsole.MarkupLine("[grey]... (content truncated)[/]");
                }
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error reading file:[/] {ex.Message}");
            }

            AnsiConsole.Write(new Rule());
        }
    }
}