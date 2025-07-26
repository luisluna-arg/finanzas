namespace CQRSDispatch.Interfaces
{
    /// <summary>
    /// Interface for commands and queries that can receive an execution context.
    /// </summary>
    /// <typeparam name="TContext">The type of dispatch context.</typeparam>
    public interface IContextAware<TContext>
        where TContext : DispatchContext
    {
        /// <summary>
        /// Sets the execution context for the command or query.
        /// </summary>
        /// <param name="context">The execution context.</param>
        void SetContext(TContext context);
    }
}
