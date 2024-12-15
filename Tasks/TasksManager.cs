using System.Text.Json;
using jukumu.Conversations;
using jukumu.InputOutput;
using jukumu.Tasks;

namespace Jukumu.Tasks
{
    public class TasksManager
    {
        public string TasksDirectory
        {
            get => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Statics.AppName);
        }


        public List<TaskDescription> AvailableTasks
        {
            get
            {
                Func<string, string> getManifestFilename = (string directoryName) =>
                {
                    var finalPath = Path.Combine(directoryName, Statics.AppName, "manifest.json");
                    return finalPath;
                };

                EnsureTasksDirectoryExists();
                var results = Directory.GetDirectories(TasksDirectory)
                .Where(a => File.Exists(getManifestFilename(a)))
                .Select(a =>
                {
                    try
                    {
                        var manifestContent = File.ReadAllText(getManifestFilename(a));
                        var taskDescription = System.Text.Json.JsonSerializer.Deserialize<TaskDescription>(
                            manifestContent
                        );
                        if (taskDescription != null)
                        {
                            PopulateActions(taskDescription, a);
                        }
                        return taskDescription;
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error reading manifest at '{a}'");
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            Console.ResetColor();
                        }
                        return null;
                    }
                });
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return results.Where(a => a != null).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
        }

        private void PopulateActions(TaskDescription taskDescription, string directoryName)
        {
            var actionsDirectory = Path.Combine(directoryName, Statics.AppName, "Actions");
            if (Directory.Exists(actionsDirectory) == false)
            {
                return;
            }
            var actionFiles = Directory.GetFiles(actionsDirectory, "*.json");
            foreach (var specificActionFile in actionFiles)
            {
                try
                {
                    var fileContent = File.ReadAllText(specificActionFile);
                    var taskAction = JsonSerializer.Deserialize<TaskAction>(fileContent);
                    if (taskAction != null)
                    {
                        taskDescription.Actions.Add(taskAction);
                    }
                }
                catch (Exception e)
                {
                    Writer.WriteError($"Error loading action file '{specificActionFile}'\n{e.Message}");
                }

            }
        }

        private void EnsureTasksDirectoryExists()
        {
            if (!Directory.Exists(TasksDirectory))
            {
                Directory.CreateDirectory(TasksDirectory);
            }
        }
    }
}