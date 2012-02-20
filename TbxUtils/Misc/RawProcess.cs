using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Tbx.Utils
{
    public class RawProcess
    {
        private static void CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            bool bInheritHandles,
            uint dwCreationFlags,
            ref Syscalls.STARTUPINFO lpStartupInfo,
            out Syscalls.PROCESS_INFORMATION lpProcessInformation)
        {
            int retval;
            retval = Syscalls._CreateProcess(
                lpApplicationName,
                lpCommandLine,
                new IntPtr(0), new IntPtr(0),
                bInheritHandles,
                dwCreationFlags,
                new IntPtr(0), null,
                ref lpStartupInfo,
                out lpProcessInformation);

            if (retval == 0)
            {
                int errcode = Marshal.GetLastWin32Error();
                throw new Exception("Cannot create process (" + errcode + ": " + Syscalls.GetLastErrorStringMessage() + ")");
            }
        }

        private string m_CommandLine;
        private Syscalls.STARTUPINFO m_startupInfo = new Syscalls.STARTUPINFO();
        private Syscalls.PROCESS_INFORMATION m_processInfo = new Syscalls.PROCESS_INFORMATION();
        private bool m_inheritHandles = false;
        private uint m_creationFlags = (int)Syscalls.CREATION_FLAGS.NORMAL_PRIORITY_CLASS;
        private bool m_running = false;

        public event EventHandler<EventArgs> ProcessEnd;

        private delegate void ProcessDelegate();

        /* Quote your paths if they contain spaces */
        public string CommandLine
        {
            get { return m_CommandLine; }
            set
            {
                if (value[0] == '"')
                    m_CommandLine = value;
                else
                {
                    string[] splited = value.Split(new char[] { ' ' }, 2);
                    string progname;
                    string arguments = "";
                    switch (splited.Length)
                    {
                        case 1:
                            progname = splited[0];
                            break;
                        case 2:
                            progname = splited[0];
                            arguments = splited[1];
                            break;
                        default:
                            throw new Exception("Invalid command line specified");
                    }


                    m_CommandLine = "\"" + progname + "\"" + arguments;
                }
            }
        }

        public Syscalls.STARTUPINFO StartupInfo
        {
            get { return m_startupInfo; }
            set { m_startupInfo = value; }
        }

        private Syscalls.PROCESS_INFORMATION ProcessInfo
        {
            get { return m_processInfo; }
        }

        public bool InheritHandles
        {
            get { return m_inheritHandles; }
            set { m_inheritHandles = value; }
        }

        public uint CreationFlags
        {
            get { return m_creationFlags; }
            set { m_creationFlags = value; }
        }

        public bool IsRunning
        {
            get { return m_running; }
        }

        public RawProcess(string commandline)
        {
            CommandLine = commandline;
        }

        public class ProcEndEventArgs : EventArgs
        {
            private int m_exitCode;
            public int ExitCode
            {
                get { return m_exitCode; }
            }

            public ProcEndEventArgs(int exitCode)
            {
                m_exitCode = exitCode;
            }
        }

        private void DoStart()
        {
            uint reason = Syscalls.WaitForSingleObject(ProcessInfo.hProcess, Syscalls.INFINITE);
            Debug.Assert(reason == (uint)Syscalls.WAIT_RETURN_REASON.WAIT_OBJECT_0);
            m_running = false;
            int exitCode = 0;
            Syscalls.GetExitCodeProcess(ProcessInfo.hProcess, ref exitCode);
            if (ProcessEnd != null)
                ProcessEnd((object)this, new ProcEndEventArgs(exitCode));
        }

        public void Start()
        {
            ProcessDelegate proc = new ProcessDelegate(DoStart);

            m_processInfo = new Syscalls.PROCESS_INFORMATION();
            CreateProcess(null, CommandLine, InheritHandles, CreationFlags, ref m_startupInfo, out m_processInfo);
            m_running = true;
            proc.BeginInvoke(null, null);
        }

        public void Terminate()
        {
            Terminate(256);
        }
        public void Terminate(int code)
        {
            Syscalls.TerminateProcess(ProcessInfo.hProcess, code);
        }
    }
}
