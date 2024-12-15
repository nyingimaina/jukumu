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
            new ProcessRunner(executionResults)
                .Run(executable: globalExecutable, taskAction: taskAction);
        }

        private static void ExecutePlan(TaskAction taskAction)
        {
            var executionResults = new Dictionary<string, string>();
            const string divider = "------------------------------------------------------------";
            var counter = 0;
            foreach (var specificExchange in taskAction.Conversation.Exchange)
            {
                specificExchange.Answer = Context.GetWithVariablesReplaced(specificExchange.Answer, taskAction.Conversation);
            }
            
            foreach (var action in taskAction.)
            {
                action.Key = Context.GetWithVariablesReplaced(action.Key, taskAction.Conversation);

                Writer.WriteInfo($"{divider}\n\t{++counter}. Attempting Command: {action.Key}\n{divider}\n");


                using (var processRunner = new ProcessRunner(executionResults))
                {
                    processRunner.SetArgumentQuotingPreference(plan.QuoteSpacedArguments);
                    foreach (var arg in action.Args)
                    {
                        var trueArg = Context.GetWithVariablesReplaced(arg, plan.Conversation);
                        processRunner.AddArgument(trueArg);
                    }
                    processRunner.DoNotUseShellExecute();
                    processRunner.SetOutputFuncs(LogWrapper.Debug, LogWrapper.Error);
                    processRunner.Run(action, plan.Conversation);
                }
                LogWrapper.Debug($"\n{divider}\nSucceeded: {action.Name}\n{divider}\n\n\n");
            }
        }
    }
}