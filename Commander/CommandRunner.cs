using jukumu.AppContext;
using jukumu.Conversations;
using jukumu.InputOutput;
using jukumu.Tasks;

namespace Jukumu.Commander
{
    public static class CommandRunner
    {
        public static void Run(
            string globalExecutable,
            TaskAction taskAction)
        {
            Console.WriteLine($"Running {taskAction.Key}");
            var executionResults = new Dictionary<string, string>();
            Talker.Talk(taskAction.Conversation);
            ExecuteAction(taskAction);
        }

        private static void ExecuteAction(TaskAction taskAction)
        {
            var executionResults = new Dictionary<string, string>();
            const string divider = "------------------------------------------------------------";
            var counter = 0;
            foreach (var specificExchange in taskAction.Conversation.Exchange)
            {
                specificExchange.Answer = Context.GetWithVariablesReplaced(specificExchange.Answer, taskAction.Conversation);
            }
            
            foreach (var command in taskAction.Commands)
            {
                command.Key = Context.GetWithVariablesReplaced(command.Key, taskAction.Conversation);

                Writer.WriteInfo($"{divider}\n\t{++counter}. Attempting Command: {command.Key}\n{divider}\n");


                using (var processRunner = new ProcessRunner(executionResults))
                {
                    processRunner.SetArgumentQuotingPreference(true);
                    foreach (var arg in command.Arguments)
                    {
                        var trueArg = Context.GetWithVariablesReplaced(arg, taskAction.Conversation);
                        processRunner.AddArgument(trueArg);
                    }
                    processRunner.DoNotUseShellExecute();
                    processRunner.SetOutputFuncs(Writer.WriteInfo, Writer.WriteError);
                    processRunner.Run(command);
                }
                Writer.WriteInfo($"\n{divider}\nSucceeded: {command.Key}\n{divider}\n\n\n");
            }
        }
    }
}