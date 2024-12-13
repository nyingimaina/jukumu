using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jukumu.Tasks
{
    public class TaskDescription : CommandDescription
    {
        
        public HashSet<ExecutableCommand> Commands { get; set; } = new HashSet<ExecutableCommand>();

    }
}