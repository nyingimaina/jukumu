using jukumu.Conversations;

namespace jukumu.Tasks
{
    public class TaskDescription : CommandDescription
    {

        public HashSet<TaskAction> Actions { get; set; } = new HashSet<TaskAction>();

    }
}