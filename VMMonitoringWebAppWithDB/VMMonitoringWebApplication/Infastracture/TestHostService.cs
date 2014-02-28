namespace VMMonitoringWebApplication.Infastracture
{
    using System;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;
    using System.Management.Automation.Runspaces;

    public class TestHostService : IHostService
    {
        private string vmmServerName;
        private readonly IProvider<PSCommand> psProvider;

        public TestHostService(IProvider<PSCommand> psProvider)
        {
            this.psProvider = psProvider;
        }

        public async Task<IEnumerable<VMItem>> GetVMListAsync(string name)
        {
            var command = new PSCommand { ScriptCommand = String.Format("Get-Service -ComputerName {0}", name) };
            
            var objects = await psProvider.ExecuteAsync<PSObject>(command);

            var mapper = new TestMapper();
            return objects.Select(mapper.Map);
        }
    }
}