namespace Lumione.Invokers
{
    /// <summary>
    /// Interface for an Invoker that acts on a command.
    /// </summary>
    public interface IInvoker
    {
        /// <summary>
        /// Acts on the given command and executes it.
        /// Implemented by classes inheriting this interface.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The result of executing the command.</returns>
        string Invoke(string command);

        /// <summary>
        /// Checks if the given command can be invoked by the current class inheriting from this interface.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>True if the command can be executed.</returns>
        bool CanInvoke(string command);
    }
}