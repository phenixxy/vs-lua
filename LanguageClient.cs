using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Task = System.Threading.Tasks.Task;

namespace VSLua
{
    [ContentType("lua")]
    [Export(typeof(ILanguageClient))]
    public class LanguageClient : ILanguageClient
    {
        #region ILanguageClient

        public string Name => "Lua Language Extension";
        public IEnumerable<string> ConfigurationSections => null;
        public object InitializationOptions => null;
        public IEnumerable<string> FilesToWatch => null;

        public event AsyncEventHandler<EventArgs> StartAsync;
        public event AsyncEventHandler<EventArgs> StopAsync;

        public async Task<Connection> ActivateAsync(CancellationToken token)
        {
            await Task.Yield();

            var serverPath = GetLuaLanguageServerPath();
            if (serverPath == null)
            {
                return null;
            }

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(serverPath, @"bin\Windows\lua-language-server.exe"),
                    Arguments = "-E main.lua",
                    WorkingDirectory = serverPath,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };
            if (process.Start())
            {
                return new Connection(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);
            }

            return null;
        }

        public async Task OnLoadedAsync()
        {
            await StartAsync?.InvokeAsync(this, EventArgs.Empty);
        }

        public Task OnServerInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnServerInitializeFailedAsync(Exception e)
        {
            return Task.CompletedTask;
        }

        #endregion

        private string GetLuaLanguageServerPath()
        {
            var user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var extensions = Path.Combine(user, @".vscode\extensions");
            if (Directory.Exists(extensions))
            {
                var dirs = Directory.GetDirectories(extensions, "sumneko.lua-*");
                if (dirs != null && dirs.Length > 0)
                {
                    Array.Sort(dirs);
                    var path = Path.Combine(dirs.Last(), @"server\bin\Windows\lua-language-server.exe");
                    if (File.Exists(path))
                    {
                        return Path.Combine(dirs.Last(), "server"); ;
                    }
                }
            }
            return null;
        }
    }
}
