using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;

namespace Tbx.Utils
{
    /// <summary>
    /// Base utility class.
    /// </summary>
    public static class Base
    {
        /// <summary>
        /// Empty delegate.
        /// </summary>
        public delegate void EmptyDelegate();

        /// <summary>
        /// Event handler delegate.
        /// </summary>
        public delegate void EventHandlerDelegate(Object sender, EventArgs args);

        /// <summary>
        /// Exception-reporting delegate.
        /// </summary>
        public delegate void ExceptionDelegate(Exception ex);

        /// <summary>
        /// General error handling delegate.
        /// </summary>
        public delegate void HandleErrorDelegate(String errorMessage, bool fatalFlag);

        /// <summary>
        /// UI control used for BeginInvoke() calls.
        /// </summary>
        public static Control InvokeUiControl = null;

        /// <summary>
        /// Fired when an error must be handled.
        /// </summary>
        public static HandleErrorDelegate HandleErrorCallback;

        /// <summary>
        /// Return true if a delegate can be invoked safely in the context of the UI.
        /// </summary>
        public static bool CanExecInUI()
        {
            return (InvokeUiControl != null && !InvokeUiControl.IsDisposed && !InvokeUiControl.Disposing);
        }

        /// <summary>
        /// Execute the specified delegate in the context of the UI.
        /// </summary>
        public static void ExecInUI(Delegate d)
        {
            ExecInUI(d, null);
        }

        /// <summary>
        /// Execute the specified delegate in the context of the UI.
        /// </summary>
        public static void ExecInUI(Delegate d, Object[] args)
        {
            if (!CanExecInUI()) return;
            InvokeUiControl.BeginInvoke(d, args);
        }

        /// <summary>
        /// Display an error message to the user and exit the application if 
        /// required.
        /// </summary>
        public static void HandleError(String errorMessage, bool fatalFlag)
        {
            if (HandleErrorCallback != null) HandleErrorCallback(errorMessage, fatalFlag);
        }

        /// <summary>
        /// Display an error message to the user after a non-fatal exception
        /// has been caught. 
        /// </summary>
        public static void HandleException(Exception e)
        {
            HandleException(e, false);
        }

        /// <summary>
        /// Display an error message to the user after an exception has been
        /// caught. If fatalFlag is true, the application will exit.
        /// </summary>
        public static void HandleException(Exception e, bool fatalFlag)
        {
            Logging.LogException(e);
            String msg = e.Message;
#if DEBUG
            msg += Environment.NewLine + Environment.NewLine + e.StackTrace;
#endif
            HandleError(msg, fatalFlag);
        }

        /// <summary>
        /// Return a formatted error message.
        /// </summary>
        public static String FormatErrorMsg(String msg)
        {
            return InternalFormatError(msg, true, true);
        }

        /// <summary>
        /// Return a formatted error message composed of two parts.
        /// </summary>
        public static String FormatErrorMsg(String first, String last)
        {
            return InternalFormatError(first, true, false) + ": " + InternalFormatError(last, false, true);
        }

        /// <summary>
        /// Return a formatted error message composed of two parts.
        /// </summary>
        public static String FormatErrorMsg(String first, Exception last)
        {
            return InternalFormatError(first, true, false) + ": " +
                   InternalFormatError(FormatErrorMsg(last), false, true);
        }

        /// <summary>
        /// Return a formatted error message based on the exception specified.
        /// </summary>
        public static String FormatErrorMsg(Exception ex)
        {
            if (ex.InnerException == null) return FormatErrorMsg(ex.Message);
            return FormatErrorMsg(ex.Message, ex.InnerException.Message);
        }

        /// <summary>
        /// Format the string specified with the specified leading character
        /// case and ending punctuation.
        /// </summary>
        private static String InternalFormatError(String s, bool upperFlag, bool puncFlag)
        {
            // Ensure we have a valid string.
            if (s.Length == 0) s = "unknown error";

            // Split 's' in parts and analyze.
            String first = s[0].ToString(), mid = "", last = "";
            if (s.Length > 2) mid = s.Substring(1, s.Length - 2);
            if (s.Length > 1) last = s[s.Length - 1].ToString();
            bool lastPuncFlag = (last == "." || last == "!" || last == "?");

            // Format.
            String res = upperFlag ? first.ToUpper() : first.ToLower();
            res += mid;
            if (puncFlag && !lastPuncFlag) last += ".";
            else if (!puncFlag && lastPuncFlag) last = "";
            res += last;

            return res;
        }

        public static DateTime KDateToDateTimeUTC(UInt64 _date)
        {
            DateTime date = new DateTime((long)_date * TimeSpan.TicksPerSecond);
            DateTime epochStartTime = Convert.ToDateTime("1/1/1970 00:00:00 AM");
            TimeSpan t = new TimeSpan(date.Ticks + epochStartTime.Ticks);
            return new DateTime(t.Ticks);
        }

        /// <summary>
        /// Translate a KANP timestamp to a DateTime object in LocalTime.
        /// </summary>
        public static DateTime KDateToDateTime(UInt64 _date)
        {
            return KDateToDateTimeUTC(_date).ToLocalTime();
        }

        public static bool IsNumeric(string val)
        {
            return IsNumeric(val, System.Globalization.NumberStyles.Integer);
        }

        public static bool IsNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            return Double.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        /// <summary>
        /// Return true if the string passed in parameter is a valid
        /// email address. If it is null or empty, or an invalid email
        /// address, false is returned. 
        /// </summary>
        public static bool IsEmail(string inputEmail)
        {
            if (String.IsNullOrEmpty(inputEmail)) return false;

            string strRegex = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            Regex re = new Regex(strRegex,RegexOptions.IgnoreCase);
            return re.IsMatch(inputEmail);
        }

        /// <summary>
        /// Return true if the name specified is a valid workspace name.
        /// </summary>
        public static bool IsValidKwsName(String name)
        {
            String trimmedName = name.Trim();
            if (String.IsNullOrEmpty(trimmedName)) return false;
            // FIXME : what about Greek/other weird unicode chars?
            // need a unified way of testing this, since KFS also needs it.
            Regex reg = new Regex("[\\\\/:*?\"<>|]");
            return (!reg.IsMatch(trimmedName));
        }

        /// <summary>
        /// Return a human-readable file size.
        /// </summary>
        public static string GetHumanFileSize(ulong Bytes)
        {
            StringBuilder sb = new StringBuilder(11);
            Syscalls.StrFormatByteSize((long)Bytes, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// Return the string representation of the byte array specified.
        /// </summary>
        public static String HexStr(byte[] p)
        {
            if (p == null) return "";
            char[] c = new char[p.Length * 3 + 2];
            byte b;

            c[0] = '0'; c[1] = 'x';

            for (int y = 0, x = 2; y < p.Length; ++y, ++x)
            {
                b = ((byte)(p[y] >> 4));
                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(p[y] & 0xF));
                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                c[++x] = ' ';
            }

            return new String(c);
        }

        /// <summary>
        /// Truncates the given string to a maximum of 'size' characters,
        /// including 3 trailing dots.
        /// </summary>
        public static String TroncateString(String str, int size)
        {
            if (str.Length <= size) return str;
            return str.Substring(0, size - 3) + "...";
        }

        /// <summary>
        /// Show a little Help tooltip close to the mouse cursor.
        /// </summary>
        public static void ShowHelpTooltip(String text, Control sender)
        {
            Help.ShowPopup(sender, text, new Point(Cursor.Position.X, Cursor.Position.Y + 20));
        }

        /// <summary>
        /// Return true if the byte arrays specified are equal.
        /// </summary>
        public static bool ByteArrayEqual(byte[] a1, byte[] a2)
        {
            if (a1 == null && a2 == null) return true;
            if (a1 == null || a2 == null) return false;
            if (a1.Length != a2.Length) return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Create 2 sockets connected to each other, for communication between threads.
        /// </summary>
        public static Socket[] SocketPair()
        {
            Socket Listener;
            Socket[] Pair = new Socket[2];

            // Start the listener side.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 0);
            Listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Listener.Bind(endPoint);
            Listener.Listen(1);
            IAsyncResult ServerResult = Listener.BeginAccept(null, null);

            // Connect the client to the server.
            endPoint = new IPEndPoint(IPAddress.Loopback, ((IPEndPoint)Listener.LocalEndPoint).Port);
            Pair[0] = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult ClientResult = Pair[0].BeginConnect(endPoint, null, null);

            // Get the server side socket.
            Pair[1] = Listener.EndAccept(ServerResult);
            Pair[0].EndConnect(ClientResult);

            Listener.Close();

            Pair[0].Blocking = false;
            Pair[1].Blocking = false;

            return Pair;
        }

        /// <summary>
        /// Read data from the socket specfied. Return -1 if the socket would
        /// block, otherwise return the number of bytes read.
        /// </summary>
        public static int SockRead(Socket sock, byte[] buf, int pos, int count)
        {
            if (count == 0) return 0;

            SocketError error;
            int r = sock.Receive(buf, pos, count, SocketFlags.None, out error);

            if (error == SocketError.WouldBlock) return -1;
            else if (error != SocketError.Success) throw new SocketException((int)error);
            else if (r == 0) throw new Exception("lost connection");
            return r;
        }

        /// <summary>
        /// Write data to the socket specfied. Return -1 if the socket would
        /// block, otherwise return the number of bytes written.
        /// </summary>
        public static int SockWrite(Socket sock, byte[] buf, int pos, int count)
        {
            if (count == 0) return 0;

            SocketError error;
            int r = sock.Send(buf, pos, count, SocketFlags.None, out error);

            if (error == SocketError.WouldBlock) return -1;
            else if (error != SocketError.Success) throw new SocketException((int)error);
            if (r == 0) throw new Exception("lost connection");
            return r;
        }

        public static UInt32 hton(UInt32 v)
        {
            return (UInt32)IPAddress.HostToNetworkOrder((Int32)v);
        }

        public static UInt32 ntoh(UInt32 v)
        {
            return (UInt32)IPAddress.NetworkToHostOrder((Int32)v);
        }

        public static UInt64 hton(UInt64 v)
        {
            return (UInt64)IPAddress.HostToNetworkOrder((Int64)v);
        }

        public static UInt64 ntoh(UInt64 v)
        {
            return (UInt64)IPAddress.NetworkToHostOrder((Int64)v);
        }

        /* Code taken from http://blogs.msdn.com/abhinaba/archive/2005/10/20/483000.aspx */
        public static String GetEnumDescription(Enum _en)
        {
            if (_en == null) return _en.ToString();
            Type type = _en.GetType();

            MemberInfo[] memInfo = type.GetMember(_en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(Description), false);

                if (attrs != null && attrs.Length > 0)
                    return ((Description)attrs[0]).Text;
            }

            return _en.ToString();
        }

        public class Description : Attribute
        {
            public string Text;

            public Description(string text)
            {
                Text = text;
            }
        }

        public static bool KSetForegroundWindow(IntPtr _handle)
        {
            return Syscalls.SetForegroundWindow(_handle);
        }

        public static void ShowInactiveTopmost(Form frm)
        {
            Syscalls.ShowWindow(frm.Handle, (int)Syscalls.WindowStatus.SW_SHOWNOACTIVATE);
            Syscalls.SetWindowPos(frm.Handle.ToInt32(), Syscalls.HWND_TOPMOST,
            frm.Left, frm.Top, frm.Width, frm.Height,
            Syscalls.SWP_NOACTIVATE);
        }

        public static void ScrollToBottom(IntPtr _controlHandle)
        {
            Syscalls.SendMessage(_controlHandle, (int)Syscalls.ScrollBarOptions.EM_SCROLL, (IntPtr)Syscalls.ScrollBarOptions.SB_PAGEBOTTOM, IntPtr.Zero);
        }

        /// <summary>
        /// Return the process SID, if it can be obtained. Otherwise, an empty string 
        /// is returned.
        /// </summary>
        public static String GetProcessSid(Process p)
        {
            IntPtr procTokenHandle = IntPtr.Zero;
            IntPtr pSid = IntPtr.Zero;
            String strSID = "";
            IntPtr tokenInfo = IntPtr.Zero;
            Syscalls.TOKEN_USER userToken;    
            int tokenInfoLength = 0;

            try
            {
                // Open the process in order to fill out procToken.
                if (!Syscalls.OpenProcessToken(p.Handle, Syscalls.TOKEN_QUERY, ref procTokenHandle))
                    throw Syscalls.LastErrorException();

                // Get the required buffer size by making a first call that will
                // fail, then allocate tokenInfo accordingly and make the call again.
                if (!Syscalls.GetTokenInformation(
                        procTokenHandle,
                        Syscalls.TOKEN_INFORMATION_CLASS.TokenUser,
                        tokenInfo,
                        0,
                        ref tokenInfoLength))
                {
                    // ERROR_INSUFFICIENT_BUFFER is expected. Fail on anything else.
                    if (Syscalls.GetLastError() != Syscalls.ERROR_INSUFFICIENT_BUFFER)
                        throw Syscalls.LastErrorException();
                }

                // Allocate the right buffer, now that we know the requried size.
                tokenInfo = Marshal.AllocHGlobal(tokenInfoLength);

                // Make the real call.
                if (!Syscalls.GetTokenInformation(
                        procTokenHandle,
                        Syscalls.TOKEN_INFORMATION_CLASS.TokenUser,
                        tokenInfo,
                        tokenInfoLength,
                        ref tokenInfoLength))
                {
                    throw Syscalls.LastErrorException();
                }

                userToken = (Syscalls.TOKEN_USER)Marshal.PtrToStructure(tokenInfo, typeof(Syscalls.TOKEN_USER));
                pSid = userToken.User.Sid;

                Syscalls.ConvertSidToStringSid(pSid, ref strSID);
                return strSID;
            }

            catch (Exception ex)
            {
                Logging.LogException(ex);
                return "";
            }

            finally
            {
                if (procTokenHandle != IntPtr.Zero) Syscalls.CloseHandle(procTokenHandle);
                if (tokenInfo!= IntPtr.Zero) Marshal.FreeHGlobal(tokenInfo);                
            }
        }

        /// <summary>
        /// Return the last modification date of a given file system path.
        /// If the path leads to a directory, return the creation time of the directory.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static DateTime GetLastModificationDate(string fullPath)
        {
            FileStream stream = null;
            try
            {
                if (File.Exists(fullPath))
                {
                    Syscalls.BY_HANDLE_FILE_INFORMATION bhfi;
                    stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    Syscalls.GetFileInformationByHandle(stream.SafeFileHandle.DangerousGetHandle(), out bhfi);
                    DateTime observedDate =
                        DateTime.FromFileTime((Int64)(((UInt64)bhfi.LastWriteTime.dwHighDateTime << 32) +
                                                      (UInt64)bhfi.LastWriteTime.dwLowDateTime));
                    return observedDate;
                }
                else if (Directory.Exists(fullPath))
                {
                    return new DirectoryInfo(fullPath).CreationTime.ToLocalTime();
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            catch (IOException)
            {
                // If we can't open the file directly, try going with the fileinfo method.
                try
                {
                    return new FileInfo(fullPath).LastWriteTime.ToLocalTime();
                }
                catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
        }

        public static UInt64 GetFileSize(string fullPath)
        {
            FileStream stream = null;
            try
            {
                Syscalls.BY_HANDLE_FILE_INFORMATION bhfi;
                stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                Syscalls.GetFileInformationByHandle(stream.SafeFileHandle.DangerousGetHandle(), out bhfi);
                return ((UInt64)bhfi.FileSizeHigh << 32) + bhfi.FileSizeLow;
            }
            catch (IOException)
            {
                // If we can't open the file directly, try going with the fileinfo method.
                try
                {
                    return (UInt64)new FileInfo(fullPath).Length;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
        }

        /// <summary>
        /// Hackish method used to reduce the application's working set.
        /// </summary>
        public static void MinimizeAppWorkingSet()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Syscalls.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }

        /// <summary>
        /// Copy an existing file to a new file. Overwriting a file of the same name is allowed.
        /// This method makes sure the target directory exists before making the actual copy.
        /// </summary>
        public static void SafeCopy(String source, String dest, bool overwrite)
        {
            String destDir = Path.GetDirectoryName(dest);
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            File.Copy(source, dest, overwrite);
        }

        /// <summary>
        /// KWM registry root (both in HKCU and HKLM).
        /// </summary>
        public static String GetKwmRegKeyString()
        {
            return @"Software\Teambox\Teambox Manager";
        }

        /// <summary>
        /// Open and return the user's KWM configuration registry key, 
        /// creating it if it does not already exist. Caller must make sure
        /// to close the returned value after use.
        /// </summary>
        public static RegistryKey GetKwmCURegKey()
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Base.GetKwmRegKeyString());
            if (regKey == null)
                throw new Exception("Unable to read or create the registry key HKCU\\" + Base.GetKwmRegKeyString() + ".");
            return regKey;
        }

        /// <summary>
        /// Open and return the local machine's KWM configuration registry key.
        /// If the key does not exist, throw an exception.
        /// Caller must make sure to close the returned value after use.
        /// </summary>
        public static RegistryKey GetKwmLMRegKey()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Base.GetKwmRegKeyString());
            if (key == null)
                throw new Exception("Unable to read or create the registry key HKLM\\" + Base.GetKwmRegKeyString() + ".");
            return key;
        }

        /// <summary>
        /// Outlook Connector registry root (both in HKCU and HKLM). This is
        /// not very consistent with the hierarchy but is required for 
        /// legacy purposes.
        /// </summary>
        public static String GetOtcRegKeyString()
        {
            return @"Software\Teambox\kpp-mso";
        }

        /// <summary>
        /// Open and return the user's KPP configuration registry key, 
        /// creating it if it does not already exist. Caller must make sure
        /// to close the returned value after use.
        /// </summary>
        public static RegistryKey GetOtcRegKey()
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Base.GetOtcRegKeyString());
            if (regKey == null)
                throw new Exception("Unable to read or create the registry key HKCU\\" + Base.GetOtcRegKeyString() + ".");
            return regKey;
        }

        /// <summary>
        /// Return where the KWM is installed on the file system, backslash-terminated..
        /// </summary>
        public static String GetKwmInstallationPath()
        {
            string defaultVal = @"c:\program files\teambox\Teambox Manager";

            RegistryKey kwmKey = null;
            try
            {
                kwmKey = Base.GetKwmLMRegKey();
                return (String)kwmKey.GetValue("InstallDir", defaultVal) + "\\";
            }
            catch(Exception ex)
            {
                Logging.LogException(ex);
                return defaultVal;
            }
            finally
            {
                if (kwmKey != null) kwmKey.Close();
            }
        }

        public static String GetKcsLocalDataPath()
        {
            String appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return appData + "\\teambox\\kcs\\";
        }

        public static String GetKcsRoamingDataPath()
        {
            String appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return appData + "\\teambox\\kcs\\";
        }

        public static String GetKcsLogFilePath()
        {
            return Base.GetKcsLocalDataPath() + "logs\\";
        }

        public static String GetLogFileName()
        {
            return String.Format("{0:yyyy_MM_dd-HH_mm_ss}", DateTime.Now) + ".log";
        }

        public static String GetKwmString()
        {
            return "Teambox Manager";
        }

        public static String GetKwsString()
        {
            return "Teambox";
        }

        public static String GetKwsesString()
        {
            return "Teamboxes";
        }

        /// <summary>
        /// String presented to the user when prompted for a password
        /// during workspace invitation. Used both in the KWM and the OTC.
        /// </summary>
        public static String GetPwdPromptText()
        {
            return "You are inviting people to a Secure Teambox. You must assign a password to the following users:";
        }

        public static String GetStdKwsDescription()
        {
            return "A Standard " + Base.GetKwsString() + " can be accessed by users " +
                   "who receive a " + Base.GetKwsString() + " invitation email without " +
                   "having to supply a password.";
        }

        public static String GetSecureKwsDescription()
        {
            return "A Secure " + Base.GetKwsString() + 
                   " ensures an enhanced level of security by requiring a " + 
                   "password-based authentication for every user.";
        }
    }

    /* Classes required for column sorting. Taken from
    * http://msdn.microsoft.com/en-us/library/ms229643(VS.80).aspx */

    // An instance of the SortWrapper class is created for
    // each item and added to the ArrayList for sorting.
    public class SortWrapper
    {
        internal ListViewItem sortItem;
        internal int sortColumn;


        // A SortWrapper requires the item and the index of the clicked column.
        public SortWrapper(ListViewItem Item, int iColumn)
        {
            sortItem = Item;
            sortColumn = iColumn;
        }

        // Text property for getting the text of an item.
        public string Text
        {
            get
            {
                return sortItem.SubItems[sortColumn].Text;
            }
        }

        // Implementation of the IComparer
        // interface for sorting ArrayList items.
        public class SortComparer : IComparer
        {
            bool ascending;

            // Constructor requires the sort order;
            // true if ascending, otherwise descending.
            public SortComparer(bool asc)
            {
                this.ascending = asc;
            }

            // Implemnentation of the IComparer:Compare
            // method for comparing two objects.
            public int Compare(object x, object y)
            {
                SortWrapper xItem = (SortWrapper)x;
                SortWrapper yItem = (SortWrapper)y;

                string xText = xItem.sortItem.SubItems[xItem.sortColumn].Text;
                string yText = yItem.sortItem.SubItems[yItem.sortColumn].Text;
                return xText.CompareTo(yText) * (this.ascending ? 1 : -1);
            }
        }
    }
}