using CommandDotNet;
using jukumu.InputOutput;
using Jukumu.Commander;
using Jukumu.Tasks;
using Spectre.Console;

namespace Jukumu
{
    public class Program
    {
        static int Main(string[] args)
        {
            // Create and run the CommandDotNet application
            args = [nameof(Navigator)];
            return new AppRunner<Program>().Run(args);
        }


        // Command to navigate through options
        [Command(Description = "Navigate Through Options")]
        public void Navigator()
        {
            try
            {
                while (true)
                {
                    // Fetch and display commands, excluding the Navigator command
                    var selectedCommand = NavigationManager.Command(this);

                    if (selectedCommand?.Name == "Exit")
                    {
                        AnsiConsole.MarkupLine("[green]Exiting Navigator...[/]");
                        break;
                    }

                    // Option to return to the navigator
                    if (!AnsiConsole.Confirm("[green]Do you want to return to the Navigator?[/]", true))
                    {
                        AnsiConsole.MarkupLine("[green]Exiting Navigator...[/]");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred in the Navigator:[/] {ex.Message}");
            }
        }






        [Command(Description = "Show all tasks available")]
        public void Tasks()
        {

            var tasksManager = new TasksManager();
            var tasks = tasksManager.AvailableTasks;

            if (!tasks.Any())
            {
                DisplayFeedback($"[red]There are currently no available tasks[/]");
                DisplayFeedback($"You may add tasks to {tasksManager.TasksDirectory}");
                return;
            }

            var suggestedTasks = CommandTracker.GetSuggestions(nameof(Tasks), tasks, a => a.Key);
            var selectedTaskKey = SelectionManager.SelectOption(
                suggestedTasks,
                "Select the [green]task[/] to run:",
                a => a,
                a => tasks.Single(task => task.Key == a).Description
            );


            CommandTracker.LogCommand(nameof(Tasks), selectedTaskKey);
            DisplayFeedback($"Running [blue]{selectedTaskKey}[/]...");

            var selectedTask = tasks.Single(a => a.Key == selectedTaskKey);
            var selectedCommand = SelectionManager.SelectOption(
                selectedTask.Actions,
                "Select the [green]command[/] to run:",
                keySelector: a => a.Key,
                a => a.Description
            );

            CommandRunner.Run(selectedTask.App, selectedCommand);
        }

        [Command(Description = "Manage Working Directory")]
        public void WorkingDirectory()
        {
            try
            {
                // Step 1: Display the current working directory
                var currentWorkingDirectory = Environment.CurrentDirectory;
                AnsiConsole.MarkupLine($"[blue]Current Working Directory:[/] [cyan]{currentWorkingDirectory}[/]");

                // Step 2: Ask the user if they wish to change it
                var changeDirectory = SelectionManager.Confirm("[yellow]Would you like to change the working directory?[/]", false);

                if (changeDirectory)
                {
                    // Step 3: Allow the user to change the working directory
                    var selectedPath = FileSystemBrowser.Browse(
                        initialDirectory: currentWorkingDirectory,
                        fileFilter: null,  // Not filtering files since we're dealing with directories
                        previewFiles: false);

                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        // Update the working directory
                        Environment.CurrentDirectory = selectedPath;

                        // Log the change
                        CommandTracker.LogCommand("WorkingDirectory", selectedPath);

                        // Provide feedback
                        AnsiConsole.MarkupLine($"[green]Working directory has been successfully updated to:[/] [cyan]{selectedPath}[/]");
                    }
                    else
                    {
                        // Handle the case where no directory is selected
                        AnsiConsole.MarkupLine("[red]No directory was selected. The working directory remains unchanged.[/]");
                    }
                }
                else
                {
                    // User chose not to change the working directory
                    AnsiConsole.MarkupLine("[yellow]Working directory remains unchanged.[/]");
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors gracefully
                AnsiConsole.MarkupLine($"[red]An error occurred while managing the working directory:[/] {ex.Message}");
            }
        }


        // Helper method for consistent feedback
        private static void DisplayFeedback(string message)
        {
            AnsiConsole.MarkupLine(message);
        }
    }
}
