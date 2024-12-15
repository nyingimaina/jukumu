using System.Reflection;
using jukumu.Conversations;

namespace jukumu.AppContext
{
    public static class Context
    {

        public static string AppDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;

        private static Dictionary<string, string> InbuiltVariables = new Dictionary<string, string>
        {
            {"$MarioRoot", AppDirectory},
            {"$WorkingDirectory",WorkingDirectory}
        };

        private static Dictionary<string, string> UserVariables = new Dictionary<string, string>();

        public static string WorkingDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(WorkingDirectory));
                }
                if (!Directory.Exists(value))
                {
                    throw new Exception($"Directory {value} does not exist. Cannot set this as working directory");
                }
                Environment.CurrentDirectory = value + (value.EndsWith('/') ? string.Empty : "/");
            }
        }

        public static string GetWithVariablesReplaced(string input, Conversation conversation)
        {
            foreach (var exchange in conversation.Exchange)
            {
                var variableName = "$" + exchange.Key;
                input = input.Replace($"{variableName}", exchange.Answer, StringComparison.InvariantCulture);
                foreach (var specificInbuiltVariable in Context.InbuiltVariables)
                {
                    input = input.Replace($"{specificInbuiltVariable.Key}", specificInbuiltVariable.Value, StringComparison.InvariantCulture);
                }
                foreach (var specificUserVariable in Context.UserVariables)
                {
                    input = input.Replace(specificUserVariable.Key, specificUserVariable.Value, StringComparison.InvariantCulture);
                }

            }
            return input;
        }


    }
}