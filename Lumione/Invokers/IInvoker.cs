namespace Lumione.Invokers
{
    public interface IInvoker
    {
        string Invoke(string command);

        bool CanInvoke(string command);
    }
}