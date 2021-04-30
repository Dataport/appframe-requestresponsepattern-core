using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace DotNetCoreDemo.Contracts
{
    /// <summary>
    /// ...
    /// </summary>
    public class RunDemoResponse : ResponseBase
    {
        [Required]
        [MaxLength(100)]
        public string MyGreetings { get; set; }
    }
}