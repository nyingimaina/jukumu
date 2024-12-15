using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jukumu.Conversations
{
    public class TaskCommand
    {
        public string Key { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public List<string> Arguments { get; set; } = new List<string>();

        public int SuccessExitCode { get; set; } = 0;


    }
}