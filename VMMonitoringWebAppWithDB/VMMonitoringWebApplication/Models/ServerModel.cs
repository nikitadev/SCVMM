namespace VMMonitoringWebApplication.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ServerModel
    {
        [Required]
        [Display(Name = "Server name")]
        public string Name { get; set; }
    }
}