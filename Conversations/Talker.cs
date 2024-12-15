

using jukumu.InputOutput;

namespace jukumu.Conversations
{
    public static class Talker
    {
        public static void Talk(Conversation conversation)
        {
            foreach(var exchange in conversation.Exchange)
            {
                var doesNotAlreadyHaveAnswer = string.IsNullOrEmpty(exchange.Answer);
                if(doesNotAlreadyHaveAnswer)
                {
                    exchange.Answer = SelectionManager.Ask(exchange.Question);
                }
            }
        }
    }
}