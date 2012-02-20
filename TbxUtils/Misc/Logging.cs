using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

namespace Tbx.Utils
{
    /// <summary>
    /// Every class that wants to handle some log events must implement this 
    /// interface.
    /// </summary>
    public interface ILogger
    {
        void HandleOnLogEvent(Object sender, LogEventArgs args);
    }

    /// <summary>
    /// Object passed to the log handlers when a log event is fired.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        private int m_iSeverity;
        private String m_strCaller;
        private String m_callStack;
        private String m_strLine;
        private String m_strMessage;
        private DateTime m_eventTimestamp;

        public int Severity
        {
            get
            {
                return m_iSeverity;
            }
        }

        public String Caller
        {
            get
            {
                return m_strCaller;
            }
        }

        public String CallStack
        {
            get
            {
                return m_callStack;
            }
        }

        /// <summary>
        /// Moment the event was launched. Already in LocalTime.
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return m_eventTimestamp;
            }
        }

        public String Line
        {
            get
            {
                return m_strLine;
            }
        }

        public String Message
        {
            get
            {
                return m_strMessage;
            }
        }

        public LogEventArgs(int _severity, String _caller, String _callStack,
                            String _line, String _msg, DateTime _timestamp)
        {
            m_iSeverity = _severity;
            m_strCaller = _caller;
            m_callStack = _callStack;
            m_strLine = _line;
            m_strMessage = _msg;
            m_eventTimestamp = _timestamp;
        }
    }

    public enum LoggingLevel
    {
        /// <summary>
        /// Do not log any event.
        /// </summary>
        None,

        /// <summary>
        /// Log events without their call stack.
        /// </summary>
        Normal,

        /// <summary>
        /// Log events with their call stack. This is costly.
        /// </summary>
        Debug
    }

    /// <summary>
    /// This class is used to log application events.
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Don't log more than this many events. When this number of events is
        /// reached, the number of events logged is reduced by half.
        /// </summary>
        private const UInt32 m_maxNbEvt = 10000;

        /// <summary>
        /// Current logging level. For performance reasons this is unprotected
        /// by the mutex; this is not a problem.
        /// </summary>
        private static volatile LoggingLevel m_loggingLevel = LoggingLevel.None; 

        /// <summary>
        /// Recursive mutex that serializes the access to the logging class.
        /// This can be locked outside this class to avoid synchronization issues.
        /// </summary>
        public static Object Mutex = new Object();

        /// <summary>
        /// List of buffered log events.
        /// </summary>
        private static List<LogEventArgs> m_evtList = new List<LogEventArgs>();

        /// <summary>
        /// List of log event handlers.
        /// </summary>
        private static EventHandler<LogEventArgs> m_onLogEvent;

        /// <summary>
        /// This flag is used to determine if a log handler is trying to log.
        /// </summary>
        private static bool m_firingFlag = false;

        /// <summary>
        /// Set the current logging level.
        /// </summary>
        public static void SetLoggingLevel(LoggingLevel level)
        {
            m_loggingLevel = level;
        }

        /// <summary>
        /// Register a log event listener.
        /// </summary>
        public static void RegisterLogHandler(ILogger handler)
        {
            m_onLogEvent += handler.HandleOnLogEvent;
        }

        /// <summary>
        /// Unregister a log event listener.
        /// </summary>
        public static void UnregisterLogHandler(ILogger handler)
        {
            m_onLogEvent -= handler.HandleOnLogEvent;
        }

        /// <summary>
        /// Return the list of buffered events.
        /// </summary>
        public static List<LogEventArgs> GetBufferedEventList()
        {
            lock (Mutex)
            {
                return new List<LogEventArgs>(m_evtList);
            }
        }

        /// <summary>
        /// Clear the list of buffered events.
        /// </summary>
        public static void ClearBufferedEventList()
        {
            lock (Mutex)
            {
                m_evtList.Clear();
            }
        }

        /// <summary>
        /// Log an exception.
        /// </summary>
        public static void LogException(Exception e)
        {
            privLog(2, e.ToString(), true);
        }

        /// <summary>
        /// Log a message with a severity level of 0.
        /// </summary>
        public static void Log(String _msg)
        {
            privLog(0, _msg, false);
        }

        /// <summary>
        /// Log a message with the specified level of severity.
        /// </summary>
        public static void Log(int _severity, String _msg)
        {
            privLog(_severity, _msg, false);
        }
        
        /// <summary>
        /// Handle a request to log an event. This method is private to 
        /// standardize the layout of the stack frames.
        /// </summary>
        private static void privLog(int severity, String msg, bool isLoggingException)
        {
            // Return early if we are not logging.
            if (m_loggingLevel == LoggingLevel.None) return;

            // Generate the log event.
            String callerStr = "Unknown";
            String lineStr = "?";
            String callStackStr = "";

            if (m_loggingLevel == LoggingLevel.Debug)
            {
                FormatStackFrame(new StackFrame(2, true), out callerStr, out lineStr);
                StackTrace st = new StackTrace(true);
                for (int i = 2; i < st.FrameCount; i++)
                {
                    String name, line;
                    FormatStackFrame(st.GetFrame(i), out name, out line);
                    callStackStr += name + " at line " + line + Environment.NewLine;
                }
            }

            LogEventArgs evt = new LogEventArgs(severity, callerStr, callStackStr, lineStr, msg,
                                                DateTime.Now.ToLocalTime());

            // For ordering and consistency purpose the following has to be 
            // done in mutual exclusion.
            lock (Mutex)
            {
                // Overflow, remove some events.
                if (m_evtList.Count >= m_maxNbEvt) m_evtList.RemoveRange(0, m_evtList.Count / 2);

                // Add the event to the list.
                m_evtList.Add(evt);

#if DEBUG
                Debug.WriteLine(evt.Timestamp.ToString("") + " | " + evt.Severity + " | " +
                evt.Caller + "::line " + evt.Line + " | " + evt.Message);
#endif

                // Dispatch the event to the listeners.
                if (m_onLogEvent != null && !m_firingFlag)
                {
                    try
                    {
                        m_firingFlag = true;
                        m_onLogEvent(null, evt);
                        m_firingFlag = false;
                    }

                    catch (Exception ex)
                    {
                        Base.HandleError(ex.Message, true);
                    }
                }
            }
        }

        /// <summary>
        /// Extract the name and line number of the stack frame specified, if
        /// possible.
        /// </summary>
        private static void FormatStackFrame(StackFrame frame, out String name, out String line)
        {
            MethodBase method = frame.GetMethod();

            if (method != null)
            {
                name = method.DeclaringType.Name + "::" + method.Name;
                line = frame.GetFileLineNumber().ToString();
            }

            else
            {
                name = "Unknown";
                line = "?";
            }
        }
    }
}
