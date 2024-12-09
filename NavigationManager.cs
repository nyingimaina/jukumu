using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandDotNet;
using Spectre.Console;

namespace Jukumu
{
    public static class NavigationManager
    {
        private const string Exit = "Exit";
        // Method to fetch commands and include an Exit command, excluding the 'Navigator' command
        private static object commandOwner;
        public static dynamic? Command(object commandOwner)
        {
            NavigationManager.commandOwner = commandOwner;
            var commands = typeof(Program).GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault() is CommandAttribute)
                .Where(m => m.Name != nameof(Program.Navigator))  // Exclude Navigator command
                .Select(m => new
                {
                    Name = m.Name,
                    Description = ((CommandAttribute)m.GetCustomAttribute(typeof(CommandAttribute))!).Description ?? "No description available"
                })
                .Cast<object>()
                .ToList();

            // Add Exit command
            commands.Add(new { Name = Exit, Description = "Exit the navigator." });
            return SelectAndExecuteCommand(commands);
        }

        private static dynamic? SelectAndExecuteCommand(List<dynamic> commands)
        {
            var selectedCommand = SelectionManager.SelectOption(
                commands,
                "Select a command to execute:",
                c => c.Name,
                c => c.Description
            );

            if (selectedCommand != null)
            {
                if (selectedCommand.Name != Exit)
                {
                    ExecuteCommand(selectedCommand.Name);
                }
            }

            return selectedCommand;
        }

        // Method to execute the selected command
        private static void ExecuteCommand(string commandName)
        {
            var methodInfo = typeof(Program).GetMethod(commandName);
            if (methodInfo != null)
            {
                try
                {
                    AnsiConsole.MarkupLine($"[yellow]Executing command:[/] [cyan]{commandName}[/]");
                    methodInfo.Invoke(commandOwner,null);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error while executing command:[/] {ex.Message}");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Command not found: {commandName}[/]");
            }
        }
    }
}