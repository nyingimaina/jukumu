using Jukumu.Conversations;

namespace Jukumu.Tasks
{
    public class ExecutableCommand : CommandDescription
    {
        public Conversation Conversation { get; set; }
    }
}