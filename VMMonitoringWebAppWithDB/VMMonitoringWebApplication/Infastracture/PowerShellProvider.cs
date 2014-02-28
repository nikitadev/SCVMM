using System.Diagnostics;

namespace VMMonitoringWebApplication.Infastracture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Threading.Tasks;

    public class PowerShellProvider : IProvider<PSCommand>
    {
        private bool isRunning = false;

        private readonly PSCredential psCredential;
        private readonly WSManConnectionInfo wsManConnectionInfo;

        public string Error { get; private set; }

        public PowerShellProvider(string domainUser, string pwd, string serverName)
        {
            var password = new System.Security.SecureString();
            foreach (var c in pwd.ToCharArray())
            {
                password.AppendChar(c);
            }           

            this.psCredential = new PSCredential(domainUser, password);

            string uri = String.Format("http://{0}:5985/wsman", serverName);
            this.wsManConnectionInfo = new WSManConnectionInfo(new Uri(uri), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psCredential)
            {
                AuthenticationMechanism = AuthenticationMechanism.Kerberos,
                ProxyAuthentication = AuthenticationMechanism.Negotiate
            };
        }

        public Task<IEnumerable<T>> ExecuteAsync<T>(PSCommand command)
        {
            isRunning = true;

            var result = new List<T>();
            return Task.Run(async () =>
            {
                using (var runspace = RunspaceFactory.CreateRunspace
                    (
#if !DEBUG
                    wsManConnectionInfo
#endif
                    ))
                {
                    using (var pipeline = await Task.Run(() => { runspace.Open(); return runspace.CreatePipeline(); }))
                    {
                        if (command.Commands != null)
                        {
                            var importCmd = new Command("Import-Module -Name \"virtualmachinemanager\"", true);
                            pipeline.Commands.Add(importCmd);

                            foreach (var cmd in command.Commands)
                            {
                                pipeline.Commands.Add(cmd);
                            }
                        }

                        if (!String.IsNullOrEmpty(command.ScriptCommand))
                        {
#if !DEBUG
                            var script = String.Concat("Import-Module -Name \"virtualmachinemanager\"",
                                Environment.NewLine, command.ScriptCommand);
#else
                            var script = command.ScriptCommand;
#endif
                            pipeline.Commands.AddScript(script);
                        }

                        Trace.TraceInformation(String.Join(", ", pipeline.Commands));

                        pipeline.StateChanged += ContextStateChanged;

                        pipeline.Output.DataReady += (s, e) =>
                        {
                            var output = s as PipelineReader<T>;

                            if (output != null)
                            {
                                while (output.Count > 0)
                                {
                                    var obj = output.Read();
                                    result.Add(obj);
                                }
                            }
                        };

                        pipeline.Error.DataReady += ErrorDataReady;

                        pipeline.InvokeAsync();
                        pipeline.Input.Close();

                        // ждём пока выполнится скрипт
                        while (isRunning) { await Task.Delay(5); }

                        //pipeline.Output.DataReady -= OutputDataReady;
                        pipeline.Error.DataReady -= ErrorDataReady;

                        pipeline.StateChanged -= ContextStateChanged;

                        return result.AsEnumerable();
                    }
                }
            });            
        }

        private void ContextStateChanged(object sender, PipelineStateEventArgs e)
        {
            if (e.PipelineStateInfo.State == PipelineState.Failed)
            {
                isRunning = false;

                Error = e.PipelineStateInfo.Reason.Message;
            }

            if (e.PipelineStateInfo.State == PipelineState.Completed)
            {
                isRunning = false;
            }
        }

        private void ErrorDataReady(object sender, EventArgs e)
        {
            var error = sender as PipelineReader<object>;

            var errors = new List<string>();
            while (error != null && error.Count > 0)
            {
                var psObj = (PSObject) error.Read();

                errors.Add(psObj.ToString());
            }

            Error = String.Join(Environment.NewLine, errors);
        }
    }
}