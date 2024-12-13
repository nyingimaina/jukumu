using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace Jukumu
{
    public static class SelectionManager
    {

        public static bool Confirm(string question, bool defaultValue = true)
        {
            if (AnsiConsole.Profile.Capabilities.Interactive)
            {
                return AnsiConsole.Confirm(prompt: question, defaultValue: defaultValue);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]The terminal is not interactive. Using fallback mode.[/]");
                question = RemoveMarkup(question);
                try
                {
                    var map = new Dictionary<char, bool>
                    {
                        { 'y', true},
                        { 'Y', true},
                        { 'n', false},
                        { 'N', false}
                    };

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{question} (y/n): ");
                    var input = Console.Read();
                    var inputChar = (char)input;
                    if(map.TryGetValue(inputChar, out bool value))
                    {
                        return value;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"Invalid input '{inputChar}'. Try again");
                        return Confirm(question);
                    }
                }
                finally
                {
                    Console.ResetColor();
                }
            }
        }



        public static T SelectOption<T>(
            IEnumerable<T> inputOptions,
            string title,
            Func<T, string> keySelector,
            Func<T, string> descriptionSelector)
        {
            var options = inputOptions.ToList();
            // Prepare table content
            var tableData = options.Select((item, index) => new
            {
                Index = index + 1,
                Key = keySelector(item),
                Description = descriptionSelector(item)
            }).ToList();

            // Display the table in non-interactive terminals
            var table = new Table();
            table.AddColumn("[bold]#[/]");
            table.AddColumn("[bold]Option[/]");
            table.AddColumn("[bold]Description[/]");
            table.Border(TableBorder.Rounded);

            foreach (var row in tableData)
            {
                table.AddRow(row.Index.ToString(), row.Key, row.Description);
            }

            AnsiConsole.Write(table);

            // Handle input selection for interactive terminals
            if (AnsiConsole.Profile.Capabilities.Interactive)
            {
                // Interactive terminal: use SelectionPrompt with typeahead filtering (built-in)
                var selectionPrompt = new SelectionPrompt<string>()
                        .Title($"[yellow]{title}[/]")
                        .PageSize(10)
                        .AddChoices(tableData.Select(row => row.Key).ToArray());
                selectionPrompt.SearchEnabled = true;
                var selection = AnsiConsole.Prompt(
                     selectionPrompt
                );

                // Find the selected option based on the key
                return options.FirstOrDefault(option => keySelector(option) == selection)!;
            }
            else
            {
                // Non-interactive terminal: fallback to number input
                AnsiConsole.MarkupLine("[red]The terminal is not interactive. Using fallback mode.[/]");

                int selectedIndex;
                do
                {

                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Enter the number of the option to select: ");
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                    var input = Console.ReadLine();
                    selectedIndex = int.TryParse(input, out int result) && result > 0 && result <= tableData.Count
                        ? result - 1
                        : -1;

                    if (selectedIndex == -1)
                    {
                        AnsiConsole.MarkupLine("[red]Invalid selection. Please try again.[/]");
                    }
                } while (selectedIndex == -1);

                return options[selectedIndex];
            }

            
        }


        private static string RemoveMarkup(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Regular expression to match AnsiConsole markup, which is enclosed in square brackets
            var markupPattern = @"\[.*?\]";
            
            // Replace the markup tags with an empty string
            return System.Text.RegularExpressions.Regex.Replace(value, markupPattern, string.Empty);
        }
        

    }
}
