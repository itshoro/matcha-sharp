using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione.Invokers
{
    public abstract class IInvoke
    {
        protected SettingsManager SettingsManager;
        protected IInvoke(SettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }
        public abstract string Invoke(string comment, string filePath);
    }
}
