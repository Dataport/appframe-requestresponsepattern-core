namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Request ist mit verfahrensspezifischen technischen Hinweisen ausgestattet.
    /// </summary>
    public interface IHasRequestAnnotation
    {
        /// <summary>
        /// Verfahrensspezifische technische Hinweise.
        /// </summary>
        /// <returns>technische Hinweise</returns>
        string RequestAnnotation { get; set; }
    }
}
