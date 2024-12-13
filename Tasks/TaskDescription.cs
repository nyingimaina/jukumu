using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jukumu.Conversations;

namespace Jukumu.Tasks
{
    public class TaskDescription : CommandDescription
    {

        public HashSet<TaskAction> Actions { get; set; } = new HashSet<TaskAction>();

    }
}