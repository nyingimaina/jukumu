using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace jukumu.InputOutput
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
                var map = new Dictionary<char, bool>
                {
                    { 'y', true },
                    { 'Y', true },
                    { 'n', false },
                    { 'N', false }
                };

                question = RemoveMarkup(question);
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{question} (y/n): ");

                    var input = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(input) && map.TryGetValue(char.ToLowerInvariant(input[0]), out bool value))
                    {
                        return value;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Try again.");
                    Console.ResetColor();
                }
            }
        }

        public static string Ask(string question)
        {
            if (AnsiConsole.Profile.Capabilities.Interactive)
            {
                return AnsiConsole.Ask<string>(prompt: question);
            }
            else
            {
                 try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(question);
                }
                finally
                {
                    Console.ResetColor();
                }
                return Console.ReadLine() ?? string.Empty;
            }
        }

        public static T SelectOption<T>(
            IEnumerable<T> inputOptions,
            string title,
            Func<T, string> keySelector,
            Func<T, string> descriptionSelector)
        {
            if (inputOptions == null)
            {
                throw new ArgumentNullException(nameof(inputOptions), "Input options cannot be null.");
            }

            var options = inputOptions.ToList();

            if (!options.Any())
            {
                throw new ArgumentException("Input options cannot be empty.", nameof(inputOptions));
            }

            // Add a "Cancel" option as the first item with a unique key
            var cancelOptionKey = "Cancel";
            var cancelOptionDescription = "Go back or cancel.";
            var cancelOptionIndex = 0;

            try
            {
                // Generate table data with the "Cancel" option
                var tableData = options.Select((item, index) => new
                {
                    Index = index + 1,
                    Key = keySelector(item) ?? "Invalid Key",
                    Description = descriptionSelector(item) ?? "No Description Available"
                }).ToList();

                tableData.Insert(0, new
                {
                    Index = cancelOptionIndex,
                    Key = cancelOptionKey,
                    Description = cancelOptionDescription
                });

                // Display table
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

                if (AnsiConsole.Profile.Capabilities.Interactive)
                {
                    var selectionPrompt = new SelectionPrompt<string>()
                        .Title($"[yellow]{title}[/]")
                        .PageSize(10)
                        .AddChoices(tableData.Select(row => row.Key).Distinct().ToArray());
                    selectionPrompt.SearchEnabled = true;
                    var selection = AnsiConsole.Prompt(selectionPrompt);

                    if (selection == cancelOptionKey)
                    {
                        return default!; // Return default if "Cancel" is chosen
                    }

                    return options.FirstOrDefault(option => keySelector(option) == selection)!;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]The terminal is not interactive. Using fallback mode.[/]");

                    int selectedIndex;
                    do
                    {
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Enter the number of the option to select: ");
                            Console.ResetColor();

                            var input = Console.ReadLine();
                            selectedIndex = int.TryParse(input, out int result) && result >= 0 && result < tableData.Count
                                ? result
                                : -1;

                            if (selectedIndex == -1)
                            {
                                AnsiConsole.MarkupLine("[red]Invalid selection. Please try again.[/]");
                            }
                        }
                        catch (Exception)
                        {
                            selectedIndex = -1;
                            AnsiConsole.MarkupLine("[red]Error processing input. Please try again.[/]");
                        }
                    } while (selectedIndex == -1);

                    if (selectedIndex == cancelOptionIndex)
                    {
                        return default!; // Return default if "Cancel" is chosen
                    }

                    return options[selectedIndex - 1];
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error generating table data. Check your selectors.", ex);
            }
        }

        private static string RemoveMarkup(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var markupPattern = @"\[.*?\]";

            try
            {
                return Regex.Replace(value, markupPattern, string.Empty);
            }
            catch (Exception)
            {
                return value; // Fallback to returning original value if regex fails
            }
        }
    }
}
