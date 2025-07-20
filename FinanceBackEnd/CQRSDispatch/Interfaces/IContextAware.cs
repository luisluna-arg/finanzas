namespace CQRSDispatch.Interfaces
{
    /// <summary>
    /// Interface for commands that can receive an execution context
    /// </summary>
    public interface IContextAware
    {
        /// <summary>
        /// Sets the execution context for the command
        /// </summary>
        /// <param name="context">The execution context</param>
        void SetContext(ExecutionContext context);
    }
}
