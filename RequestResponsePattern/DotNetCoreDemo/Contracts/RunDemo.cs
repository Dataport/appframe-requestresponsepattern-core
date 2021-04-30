using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace DotNetCoreDemo.Contracts
{
    /// <summary>
    /// ...
    /// </summary>
    public class RunDemo : RequestBase<RunDemoResponse>
    {
        [Required]
        [MaxLength(20)]
        public string MyHello { get; set; }
    }
}
