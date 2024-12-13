using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jukumu.Tasks;

namespace Jukumu.Commander
{
    public static class CommandRunner
    {
        public static void Run(ExecutableCommand commandDescription)
        {
            Console.WriteLine($"Running {commandDescription.Name}");
        }
    }
}