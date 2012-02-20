using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Tbx.Utils
{
    /// <summary>
    /// Excellent discussion about timers in .NET here :
    /// http://msdn.microsoft.com/en-us/magazine/cc164015.aspx
    /// </summary>
    public class KWakeupTimer
    {
        private System.Threading.Timer m_timer;

        public delegate void OnTimerWakeUpDelegate(object[] args);

        /// <summary>
        /// Callback invoked in the UI thread on every timer tick.
        /// </summary>
        public OnTimerWakeUpDelegate TimerWakeUpCallback;

        public Object[] Args = null;

        public KWakeupTimer()
        {
            m_timer = new System.Threading.Timer(
                new TimerCallback(HandleOnTimerElapsed),
                null,
                Timeout.Infinite, 
                Timeout.Infinite);
        }

        /// <summary>
        /// Wake me in x milliseconds. 
        /// 0 to wake me ASAP, -1 to disable.
        /// </summary>
        public void WakeMeUp(long milliseconds)
        {
            // If the timer is already enabled when the Start 
            // method is called, the interval is reset. 
            if (milliseconds == -1)
                m_timer.Change(Timeout.Infinite, Timeout.Infinite);
            else
                m_timer.Change(milliseconds, Timeout.Infinite);
        }

        private void HandleOnTimerElapsed(object source)
        {
            try
            {
                if (TimerWakeUpCallback != null) Base.ExecInUI(TimerWakeUpCallback, new object[] {Args});
            }

            catch (Exception ex)
            {
                Logging.LogException(ex);
            }
        }
    }
}
