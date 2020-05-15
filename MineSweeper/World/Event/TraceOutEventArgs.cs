using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.World
{
    public delegate void TraceOutEventHandler(object sender, TraceOutEventArgs e);

    public class TraceOutEventArgs : EventArgs
    {
        public string Message { get; private set; } = string.Empty;

        public TraceOutEventArgs(string aMessage)
        {
            Message = aMessage;
        }

        public TraceOutEventArgs(string aFromat, params object [] aArgs)
        {
            Message = string.Format(aFromat, aArgs);
        }
    }
}
