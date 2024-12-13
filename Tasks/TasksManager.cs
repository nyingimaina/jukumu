using System.Text.Json.Serialization;

namespace Jukumu.Tasks
{
    public class TasksManager
    {
        public string TasksDirectory
        {
            get => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jukumu");
        }


        public List<TaskDescription> AvailableTasks
        {
            get
            {
                Func<string, string> getManifestFilename = (string directoryName) =>
                {
                    return Path.Combine(directoryName, "jukumu.json");
                };

                EnsureTasksDirectoryExists();
                var results = Directory.GetDirectories(TasksDirectory)
                .Where(a => File.Exists(getManifestFilename(a)))
                .Select(a =>
                {
                    try
                    {
                        var manifestContent = File.ReadAllText(getManifestFilename(a));
                        return System.Text.Json.JsonSerializer.Deserialize<TaskDescription>(
                            manifestContent
                        );
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


        private void EnsureTasksDirectoryExists()
        {
            if(!Directory.Exists(TasksDirectory))
            {
                Directory.CreateDirectory(TasksDirectory);
            }
        }
    }
}