using System;
using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    public abstract class InvokerBase : IInvoker
    {
        protected string pattern;
        protected Settings settings;
        public FileContext Context { get; private set; }

        protected InvokerBase(Settings settings)
        {
            this.settings = settings;
            Context = new FileContext();
        }

        public virtual bool CanInvoke(string command)
        {

            return Regex.IsMatch(command, pattern);
        }

        public abstract string Invoke(string command);
    }
}