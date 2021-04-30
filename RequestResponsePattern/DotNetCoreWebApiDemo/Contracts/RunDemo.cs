using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace DotNetCoreWebApiDemo.Contracts
{
    /// <summary>
    /// ...
    /// </summary>
    public class RunDemo : RequestBase<RunDemoResponse>
    {
        [Required]
        [MaxLength(100)]
        public string MyHello { get; set; }
    }
}
