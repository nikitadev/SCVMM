namespace VMMonitoringWebApplication.Infastracture
{
    using System.Collections.Generic;
    using System.Management.Automation.Runspaces;

    public class PSCommand
    {
        public string ScriptCommand { get; set; }

        public IList<Command> Commands { get; set; }
    }
}