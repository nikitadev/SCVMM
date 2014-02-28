using System.Diagnostics;
using System.Management.Automation.Runspaces;

namespace VMMonitoringWebApplication.Infastracture
{
    using System.Web;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using System.Management.Automation;

    public sealed class HostService : IHostService
    {
        private string vmmServerName;
        private readonly IProvider<PSCommand> psProvider;

        public HostService(IProvider<PSCommand> psProvider)
        {
            this.psProvider = psProvider;
        }

        public void SetServer(string vmmServerName)
        {
            this.vmmServerName = vmmServerName;
        }

        public async Task<IEnumerable<VMItem>> GetVMListAsync(string name)
        {
            var psCommand = new PSCommand
            {
                Commands = new List<Command>()
            };

            if (!String.IsNullOrEmpty(vmmServerName))
            {
                psCommand.Commands.Add(new Command(String.Format("$Cloud = Get-SCCloud -Name \"{0}\" -VMMServer \"{1}\"", name, vmmServerName), true));
                psCommand.Commands.Add(new Command("Get-SCVirtualMachine -Cloud $Cloud", true));

                //command = String.Concat(String.Format("$Cloud = Get-SCCloud -Name \"{0}\" -VMMServer \"{1}\"", name, vmmServerName), Environment.NewLine, "Get-SCVirtualMachine -Cloud $Cloud");
            }
            else
            {
                psCommand.ScriptCommand = String.Format("Get-SCVirtualMachine -VMMServer \"{0}\"", name);
            }

            var objects = await psProvider.ExecuteAsync<PSObject>(psCommand);
            if (!String.IsNullOrEmpty(psProvider.Error))
            {
                throw new Exception(psProvider.Error);
            }

            var mapper = new PSMapper();
            return objects.Select(mapper.Map);
        }
    }
}