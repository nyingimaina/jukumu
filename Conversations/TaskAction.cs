using System.Collections.ObjectModel;
using jukumu.Conversations;

namespace jukumu.Conversations
{
    public class TaskAction
    {
        public Conversation Conversation { get; set; } = new Conversation();
        public string Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<TaskCommand> Commands { get; set; } = new List<TaskCommand>();

        public override bool Equals(object? obj)
        {
            if (obj is TaskAction other)
            {
                return Conversation.Equals(other.Conversation) &&
                       Key == other.Key &&
                       Description == other.Description;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Conversation, Key, Description);
        }
    }

}