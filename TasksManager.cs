namespace Jukumu
{
    public class TasksManager
    {
        public string TasksDirectory
        {
            get => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Jukumu");
        }


        public List<string> AvailableTasks
        {
            get
            {
                EnsureTasksDirectoryExists();
                return Directory.GetDirectories(TasksDirectory).ToList();
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