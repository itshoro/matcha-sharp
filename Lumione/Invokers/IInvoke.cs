using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione.Invokers
{
    public abstract class IInvoke
    {
        public abstract string Invoke(string comment, string filePath);
    }
}
