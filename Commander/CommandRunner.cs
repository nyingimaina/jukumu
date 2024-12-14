using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jukumu.Conversations;
using Jukumu.Conversations;
using Jukumu.Tasks;

namespace Jukumu.Commander
{
    public static class CommandRunner
    {
        public static void Run(string globalExecutable, TaskAction taskAction)
        {
            Console.WriteLine($"Running {taskAction.Key}");
            var executionResults = new Dictionary<string, string>();
            Talker.Talk(taskAction.Conversation);
            new ProcessRunner(executionResults)
                .Run(executable: globalExecutable, taskAction: taskAction);
        }

        private static void ExecutePlan(Conversation conversation)
        {
            var executionResults = new Dictionary<string, string>();
            const string divider = "------------------------------------------------------------";
            var counter = 0;
            foreach (var specificExchange in conversation.Exchange)
            {
                specificExchange.Answer = Context.GetWithVariablesReplaced(specificExchange.Answer, plan.Conversation);
            }
            foreach (var command in plan.Commands)
            {
                using (new Profiler(command.Name))
                {
                    command.Name = Context.GetWithVariablesReplaced(command.Name, plan.Conversation);

                    LogWrapper.Debug($"{divider}\n\t{++counter}. Attempting Command: {command.Name}\n{divider}\n");


                    using (var processRunner = new ProcessRunner(executionResults))
                    {
                        processRunner.SetArgumentQuotingPreference(plan.QuoteSpacedArguments);
                        foreach (var arg in command.Args)
                        {
                            var trueArg = Context.GetWithVariablesReplaced(arg, plan.Conversation);
                            processRunner.AddArgument(trueArg);
                        }
                        processRunner.DoNotUseShellExecute();
                        processRunner.SetOutputFuncs(LogWrapper.Debug, LogWrapper.Error);
                        processRunner.Run(command, plan.Conversation);
                    }
                    LogWrapper.Debug($"\n{divider}\nSucceeded: {command.Name}\n{divider}\n\n\n");
                }
            }
        }
    }
}