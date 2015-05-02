using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace BaseLib
{
    public class Events
    {
        public event EventHandler addToLogger;

        public void LogText(EventsArgs e)
        {
            if (addToLogger != null)
            {
                addToLogger(this, e); //Fires the event
            }
        }

        public event EventHandler incrementCount;

        public void IncreaseCounter(EventsArgs e)
        {
            if (incrementCount != null)
            {
                incrementCount(this, e);
            }
        }
    }

    public class EventsArgs : EventArgs
    {
        public string log { get; set; }

        public EventsArgs(string Log)
        {
            this.log = Log;
        }

        //public bool setTrueFalse { get; set; }

        //public EventsArgs(string Log, bool setTrueFalse)
        //{
        //    this.log = Log;
        //    this.setTrueFalse = setTrueFalse;
        //}

    }
}
