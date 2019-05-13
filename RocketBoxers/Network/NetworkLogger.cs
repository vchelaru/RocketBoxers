using RedGrin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketBoxers.Network
{
    public class NetworkLogger : ILogger
    {
        public static NetworkLogger Self = new NetworkLogger();

        public event EventHandler MessageAdded;

        public List<string> Messages = new List<string>();

        public LogLevels Level
        {
            get { return LogLevels.Warning; }
            set {  /* do nothing */}
        }

        public void Debug(string message)
        {
            Messages.Add(message);
            RemoveOldMessages();
            MessageAdded?.Invoke(this, null);
        }

        public void Error(string message)
        {
            Messages.Add(message);
            RemoveOldMessages();
            MessageAdded?.Invoke(this, null);
        }

        public void Info(string message)
        {
            // meh
            //Messages.Add(message);
            //RemoveOldMessages();
            //MessageAdded?.Invoke(this, null);
        }

        public void Warning(string message)
        {
            Messages.Add(message);
            RemoveOldMessages();
            MessageAdded?.Invoke(this, null);
        }

        private void RemoveOldMessages()
        {
            while (Messages.Count > 10)
            {
                Messages.RemoveAt(0);
            }
        }
    }
}
