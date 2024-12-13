using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jukumu.Conversations;
using Jukumu.Tasks;

namespace Jukumu.Commander
{
    public static class CommandRunner
    {
        public static void Run(string globalExecutable, TaskAction taskAction)
        {
            Console.WriteLine($"Running {taskAction.Key}");
            var executionResults = new Dictionary<string, string>();
            new ProcessRunner(executionResults)
                .Run(executable: globalExecutable, taskAction: taskAction);
        }
    }
}