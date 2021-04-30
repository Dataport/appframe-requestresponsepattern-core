using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace DotNetCoreWebApiDemo.Contracts
{
    /// <summary>
    /// ...
    /// </summary>
    public class RunDemoResponse : ResponseBase
    {
        [Required]
        [MaxLength(200)]
        public string MyGreetings { get; set; }
    }
}