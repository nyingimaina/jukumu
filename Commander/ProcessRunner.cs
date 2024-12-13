using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jukumu.Conversations;
using Jukumu.Conversations;
using jukumu.InputOutput;

namespace Jukumu.Commander
{
    public class ProcessRunner : IDisposable
    {
        private string _args = string.Empty;

        private Action<string, string> _fnOutput;

        private Action<string, string> _fnError;

        private bool _useShellExecute = true;

        private List<string> _cachedOutputLines = new List<string>();

        private bool _quoteSpacedArguments = false;



        public void AddArgument(string argument)
        {
            var hasSpaces = argument.IndexOf(' ') >= 0;
            if (_quoteSpacedArguments && hasSpaces)
            {
                argument = $"\"{argument}\"";
            }
            _args += $"{argument} ";
        }

        public void DoNotUseShellExecute()
        {
            _useShellExecute = false;
        }

        public void SetArgumentQuotingPreference(bool quoteSpacedArguments)
        {
            _quoteSpacedArguments = quoteSpacedArguments;
        }

        public void SetOutputFuncs(Action<string> fnOutput, Action<string> fnError)
        {
            _fnError = (commandName, errorMessage) =>
            {
                var hasErrorMessage = string.IsNullOrEmpty(errorMessage.Trim()) == false;
                if (hasErrorMessage)
                {
                    executionResults.Add(commandName, errorMessage.Trim());
                    fnError(errorMessage);
                    throw new Exception(errorMessage);
                }
            };
            _fnOutput = (commandName, output) =>
            {
                var hasOutput = string.IsNullOrEmpty(output.Trim()) == false;
                if (hasOutput)
                {
                    executionResults.Add(commandName, output.Trim());
                    fnOutput(output);
                }
            };
        }

        public void Run(string executable, TaskAction taskAction)
        {
            var conversation = taskAction.Conversation;

            _successExitCode = taskAction.SuccessExitCode;
            MakeFuncsSafe();
            _args = _args.Trim();
            Writer.WriteInfo($"Running executable: {executable}");
            Writer.WriteInfo($"Args: {_args}");
            RunExternalProcess(executable: executable, taskAction);
        }






        private void RunExternalProcess(string executable, TaskAction taskAction)
        {
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = _args,
                    CreateNoWindow = true,
                    UseShellExecute = _useShellExecute,
                    FileName = executable
                }
            })
            {
                SubscribeToEventsIfUsingShellExecute(process, taskAction.Key);
                RedirectToStandardOutputsIfNotUsingShellExecute(process);
                process.Start();
                process.WaitForExit();
                ReadOutputsIfNotUsingShellExecute(process, taskAction.Key);
                WriteLinesIfAnyCached(taskAction.Key, process.ExitCode);
            }
        }

        private void WriteLinesIfAnyCached(string commandName, int exitCode)
        {
            if (_successExitCode == null)
            {
                return;
            }
            else
            {
                var wasSuccess = exitCode == _successExitCode.Value;
                var fnWriter = wasSuccess ? _fnOutput : _fnError;
                var outputBuilder = new StringBuilder();
                foreach (var line in _cachedOutputLines)
                {
                    outputBuilder.Append($"{line}{Environment.NewLine}");
                }
                fnWriter(commandName, outputBuilder.ToString());
            }
        }

        private void CacheLine(string commandName, string line)
        {
            _ = commandName;
            _cachedOutputLines.Add(line);
        }

        private void ReadOutputsIfNotUsingShellExecute(Process process, string commandName)
        {
            if (_useShellExecute)
            {
                return;
            }
            else
            {
                while (ContentStillAvailable(process))
                {
                    RedirectStreamContentIfAvailable(process.StandardOutput, commandName, _successExitCode != null ? CacheLine : _fnOutput);
                    RedirectStreamContentIfAvailable(process.StandardError, commandName, _successExitCode != null ? CacheLine : _fnError);
                }
            }
        }

        private bool ContentStillAvailable(Process process)
        {
            return StreamHasContent(process.StandardOutput)
                || StreamHasContent(process.StandardError);
        }

        private bool StreamHasContent(StreamReader stream)
        {
            return stream.EndOfStream == false;
        }

        private void RedirectStreamContentIfAvailable(StreamReader stream, string commandName, Action<string, string> fnOut)
        {
            while (StreamHasContent(stream))
            {
                var line = stream.ReadLine();
                fnOut(commandName, line);
            }
        }

        private void RedirectToStandardOutputsIfNotUsingShellExecute(Process process)
        {
            if (_useShellExecute)
            {
                return;
            }
            else
            {
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
            }
        }

        private void SubscribeToEventsIfUsingShellExecute(Process process, string commandName)
        {
            if (_useShellExecute)
            {
                process.EnableRaisingEvents = _useShellExecute;
                if (_successExitCode == null)
                {
                    process.OutputDataReceived += (s, e) => _fnOutput(commandName, e.Data);
                    process.ErrorDataReceived += (s, e) => _fnError(commandName, e.Data);
                }
                else
                {
                    process.OutputDataReceived += (s, e) => CacheLine(commandName, e.Data);
                    process.ErrorDataReceived += (s, e) => CacheLine(commandName, e.Data);
                }
            }
        }

        private void MakeFuncsSafe()
        {
            if (_fnError == null)
            {
                _fnError = (commandName, errorMessage) => { throw new Exception(errorMessage); };
            }

            if (_fnOutput == null)
            {
                _fnOutput = (commandName, errorMessage) => { };
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private int? _successExitCode;

        private Dictionary<string, string> executionResults;

        public ProcessRunner(
            Dictionary<string, string> executionResults)
        {
            this.executionResults = executionResults;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _args = string.Empty;
                    _fnOutput = null;
                    _fnError = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Commander()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}